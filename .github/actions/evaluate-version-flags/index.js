const fs = require('fs');
const path = require('path');

const workspace = process.env.GITHUB_WORKSPACE || process.cwd();
const contextPath = path.join(workspace, 'ci-context.json');

if (!fs.existsSync(contextPath)) {
    console.error(`ci-context.json not found at ${contextPath}`);
    process.exit(1);
}

const context = JSON.parse(fs.readFileSync(contextPath, 'utf8'));
console.log('context', JSON.stringify(context, null, 2));
const flags = {};

const commonChanged = context.common_changes.licence_changed || context.common_changes.directory_build_props_changed;

for (const projName of Object.keys(context.projects)) {
    const p = context.projects[projName];

    const projectNeedsUpdate = commonChanged ||
        p.changes.code_changed ||
        p.changes.test_changed ||
        p.changes.nuget_changed ||
        p.changes.project_ref;

    flags[projName] = !!projectNeedsUpdate;
}

const flagsPath = path.join(workspace, 'version-change-flags.json');
fs.writeFileSync(flagsPath, JSON.stringify(flags, null, 2));
console.log('version-change-flags.json generated successfully.');

const matrixProjects = [];
for (const projName of Object.keys(flags)) {
    if (flags[projName]) {
        const p = context.projects[projName];
        matrixProjects.push({
            "folder": path.dirname(p.csproj_path) + "/**",
            "csproj": p.csproj_path,
            "package-id": projName,
            "artifact": "nupkg-" + projName.replace(/\./g, '-').toLowerCase()
        });
    }
}

// Write to GITHUB_OUTPUT so subsequent steps can check if tests are needed
const isNeedUnittest = Object.values(flags).some(f => f === true);
console.log('isNeedUnittest', isNeedUnittest);
console.log('matrixProjects', JSON.stringify(matrixProjects, null, 2));
if (process.env.GITHUB_OUTPUT) {
    fs.appendFileSync(process.env.GITHUB_OUTPUT, `is_need_unittest=${isNeedUnittest}\n`);

    // Output matrix JSON string with no spaces to avoid issues
    const matrixJson = JSON.stringify({ project: matrixProjects });
    fs.appendFileSync(process.env.GITHUB_OUTPUT, `matrix=${matrixJson}\n`);
}

