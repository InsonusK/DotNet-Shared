const fs = require('fs');
const { execSync } = require('child_process');
const path = require('path');

const baseRef = process.env.INPUT_BASE_REF;
if (!baseRef) {
    console.error("INPUT_BASE_REF is not defined");
    process.exit(1);
}

const workspace = process.env.GITHUB_WORKSPACE || process.cwd();

// 1. Get changed files
let changedFiles = [];
try {
    const output = execSync(`git diff --name-only ${baseRef} HEAD`, { encoding: 'utf-8', cwd: workspace });
    changedFiles = output.split('\n').map(f => f.trim()).filter(f => f.length > 0);
    console.log("Changed files:", changedFiles);
} catch (e) {
    console.error("Error getting git diff:", e.message);
    process.exit(1);
}

// 2. Discover projects
let csprojFiles = [];
try {
    const output = execSync(`find src -name "*.csproj"`, { encoding: 'utf-8', cwd: workspace });
    csprojFiles = output.split('\n').map(f => f.trim()).filter(f => f.length > 0);
} catch (e) {
    console.error("Error finding csproj:", e.message);
    process.exit(1);
}

const projects = csprojFiles.filter(f => !f.includes('.Test.'));

// Helper to get file content from git object or fs
function getFileContent(ref, filePath) {
    try {
        if (ref === 'HEAD') {
            const absolutePath = path.join(workspace, filePath);
            return fs.existsSync(absolutePath) ? fs.readFileSync(absolutePath, 'utf-8') : null;
        }
        return execSync(`git show ${ref}:${filePath}`, { encoding: 'utf-8', cwd: workspace, stdio: ['pipe', 'pipe', 'ignore'] });
    } catch {
        return null;
    }
}

function extractVersion(content) {
    if (!content) return null;
    const match = content.match(/<PackageVersion>([^<]+)<\/PackageVersion>/i);
    return match ? match[1] : null;
}

function extractNugetLibs(content) {
    if (!content) return [];
    const libs = [];
    const regex = /<PackageReference\s+Include="([^"]+)"/gi;
    let match;
    while ((match = regex.exec(content)) !== null) {
        libs.push(match[1]);
    }
    return libs;
}

function extractProjectRefs(content) {
    if (!content) return [];
    const refs = [];
    const regex = /<ProjectReference\s+Include="([^"]+)"/gi;
    let match;
    while ((match = regex.exec(content)) !== null) {
        const refPath = match[1].replace(/\\/g, '/');
        const refName = path.basename(refPath, '.csproj');
        refs.push(refName);
    }
    return refs;
}

const context = {
    projects: {},
    nuget_libs: {},
    common_changes: {
        licence_changed: changedFiles.some(f => f.toLowerCase() === 'license' || f.toLowerCase() === 'licence'),
        directory_build_props_changed: changedFiles.some(f => f.endsWith('Directory.Build.props'))
    }
};

// 3. Parse Directory.Packages.props
const dirPackagesHead = getFileContent('HEAD', 'Directory.Packages.props');
const dirPackagesBase = getFileContent(baseRef, 'Directory.Packages.props');

function parseGlobalNugets(content) {
    const result = {};
    if (!content) return result;
    const regex = /<PackageVersion\s+Include="([^"]+)"\s+Version="([^"]+)"/gi;
    let match;
    while ((match = regex.exec(content)) !== null) {
        result[match[1]] = match[2];
    }
    return result;
}

const globalLibsHead = parseGlobalNugets(dirPackagesHead);
const globalLibsBase = parseGlobalNugets(dirPackagesBase);

// Populate context.nuget_libs
Object.keys(globalLibsHead).forEach(lib => {
    const newVer = globalLibsHead[lib];
    const prevVer = globalLibsBase[lib] || null;
    if (newVer !== prevVer || prevVer) {
        context.nuget_libs[lib] = {
            prev: prevVer,
            new: newVer,
            is_changed: newVer !== prevVer
        };
    }
});

// Populate deleted/missing libs from base
Object.keys(globalLibsBase).forEach(lib => {
    if (!globalLibsHead[lib]) {
        context.nuget_libs[lib] = {
            prev: globalLibsBase[lib],
            new: null,
            is_changed: true
        };
    }
});

// 4. Process each project
for (const proj of projects) {
    const projName = path.basename(proj, '.csproj');
    const projDir = path.dirname(proj);

    // Find unittest project
    const testProjName = projName + '.Test';
    const testProj = csprojFiles.find(f => path.basename(f, '.csproj') === testProjName);

    // Check if code or test changed
    // We normalize paths to ensure matching
    const normalizedProjDir = projDir.replace(/\\/g, '/') + '/';
    const codeChanged = changedFiles.some(f => f.startsWith(normalizedProjDir));

    let testChanged = false;
    if (testProj) {
        const testProjDir = path.dirname(testProj).replace(/\\/g, '/') + '/';
        testChanged = changedFiles.some(f => f.startsWith(testProjDir));
    }

    const headContent = getFileContent('HEAD', proj);
    const baseContent = getFileContent(baseRef, proj);

    const newVer = extractVersion(headContent);
    const prevVer = extractVersion(baseContent);

    const nugetLibs = extractNugetLibs(headContent);
    const projectRefs = extractProjectRefs(headContent);

    context.projects[projName] = {
        csproj_path: proj,
        version: {
            prev: prevVer,
            new: newVer,
            is_changed: newVer !== prevVer
        },
        nuget_libs: nugetLibs,
        project_refs: projectRefs,
        changes: {
            code_changed: codeChanged,
            test_changed: testChanged,
            nuget_changed: false, // will calculate below
            project_ref: false    // will calculate below
        }
    };
}

// 5. Calculate cross-project flag dependencies
for (const projName of Object.keys(context.projects)) {
    const p = context.projects[projName];

    // check nuget changed
    p.changes.nuget_changed = p.nuget_libs.some(lib => context.nuget_libs[lib] && context.nuget_libs[lib].is_changed);

    // check project ref changed
    p.changes.project_ref = p.project_refs.some(ref => context.projects[ref] && context.projects[ref].version.is_changed);
}

const outputPath = path.join(workspace, 'ci-context.json');
console.log('context', JSON.stringify(context, null, 2));

fs.writeFileSync(outputPath, JSON.stringify(context, null, 2));
console.log('ci-context.json generated successfully.');

