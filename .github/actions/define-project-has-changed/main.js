const fs = require('fs');
const { execSync } = require('child_process');
const path = require('path');

const projectsPath = process.argv[2];
const baseRef = process.argv[3];

if (!projectsPath || !baseRef) {
    console.error("Usage: node main.js <projects.json> <base_ref>");
    process.exit(1);
}

let changedFiles = [];
try {
    console.log(`Running: git diff --name-only ${baseRef} HEAD`);
    const diffCommand = `git diff --name-only ${baseRef} HEAD`;
    console.log(`Running: ${diffCommand}`);
    const output = execSync(diffCommand, { encoding: 'utf-8' });
    changedFiles = output.split('\n').map(f => f.trim()).filter(f => f.length > 0);
    console.log("Changed files:", changedFiles);
} catch (e) {
    console.error("Error running git diff:", e.message);
    process.exit(1);
}

let projectsConfig;
try {
    projectsConfig = JSON.parse(fs.readFileSync(projectsPath, 'utf8'));
    console.log("Projects config:", projectsConfig);
} catch (e) {
    console.error("Error reading projects.json:", e.message);
    process.exit(1);
}

let propsDiffs = "";
try {
    const propsFiles = changedFiles.filter(f => f.toLowerCase().endsWith('directory.packages.props'));
    if (propsFiles.length > 0) {
        propsDiffs = execSync(`git diff ${baseRef} HEAD -- ${propsFiles.map(f => `"${f}"`).join(' ')}`, { encoding: 'utf-8' });
    }
} catch (e) {
    console.error("Error reading diff for Directory.Packages.props:", e.message);
}

const results = {};

for (const proj of projectsConfig.project) {
    const projId = proj["package-id"] || proj.folder;
    console.log("Checking project:", projId);

    console.log("Project folder:", proj.folder);
    let prefix = proj.folder.replace('**', '').replace(/^\/+/, '');
    const folderChanged = changedFiles.some(f => f.startsWith(prefix));
    console.log("Folder changed:", folderChanged);

    let nugetLibsChanged = false;
    if (proj.csproj && fs.existsSync(proj.csproj)) {
        const csprojContent = fs.readFileSync(proj.csproj, 'utf8');


        const pkgRegex = /<PackageReference\s+Include="([^"]+)"/ig;
        let match;
        while ((match = pkgRegex.exec(csprojContent)) !== null) {
            const pkgName = match[1];
            // Pattern to match modified lines (+ or -) that define a PackageVersion for the given package
            const diffRegex = new RegExp(`^[+-].*<PackageVersion[^>]+Include="${pkgName}"`, 'im');
            if (diffRegex.test(propsDiffs)) {
                nugetLibsChanged = true;
                console.log(`Package ${pkgName} changed in Directory.Packages.props for project ${projId}`);
                break;
            }
        }
    }

    const buildPropsChanged = changedFiles.some(f => f === 'Directory.Build.props' || f.endsWith('/Directory.Build.props'));

    const licenseChanged = changedFiles.some(f => f === 'LICENSE' || f.endsWith('/LICENSE'));

    let readmeChanged = false;
    if (proj.csproj && fs.existsSync(proj.csproj)) {
        const csprojContent = fs.readFileSync(proj.csproj, 'utf8');
        const match = csprojContent.match(/<None\s+Include="([^"]*readme[^"]*)"/i);
        if (match) {
            let readmePathInCsproj = match[1];
            readmePathInCsproj = readmePathInCsproj.replace(/\\/g, '/');
            const csprojDir = path.dirname(proj.csproj);
            const absoluteReadmePath = path.resolve(csprojDir, readmePathInCsproj);
            const gitRoot = path.resolve('.');
            let relativeReadmeToRepo = path.relative(gitRoot, absoluteReadmePath).replace(/\\/g, '/');

            readmeChanged = changedFiles.includes(relativeReadmeToRepo);
        } else {
            const match2 = csprojContent.match(/<PackageReadmeFile>([^<]+)<\/PackageReadmeFile>/i);
            if (match2) {
                let readmePathInCsproj = match2[1];
                readmePathInCsproj = readmePathInCsproj.replace(/\\/g, '/');
                const csprojDir = path.dirname(proj.csproj);
                const absoluteReadmePath = path.resolve(csprojDir, readmePathInCsproj);
                const gitRoot = path.resolve('.');
                let relativeReadmeToRepo = path.relative(gitRoot, absoluteReadmePath).replace(/\\/g, '/');
                readmeChanged = changedFiles.includes(relativeReadmeToRepo);
            }
        }
    }

    results[projId] = {
        folder_changed: folderChanged,
        nuget_libs_changed: nugetLibsChanged,
        build_props_changed: buildPropsChanged,
        license_changed: licenseChanged,
        readme_changed: readmeChanged
    };
}

const jsonResult = JSON.stringify(results);
console.log("JSON Result:", jsonResult);

if (process.env.GITHUB_OUTPUT) {
    fs.appendFileSync(process.env.GITHUB_OUTPUT, `changes=${jsonResult}\n`);
}
