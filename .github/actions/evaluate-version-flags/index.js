const fs = require('fs');
const path = require('path');
const core = require('@actions/core');

const workspace = process.env.GITHUB_WORKSPACE || process.cwd();
const contextPath = path.join(workspace, 'ci-context.json');

if (!fs.existsSync(contextPath)) {
    core.setFailed(`ci-context.json not found at ${contextPath}`);
    process.exit(1);
}

const context = JSON.parse(fs.readFileSync(contextPath, 'utf8'));
core.info('context: \n' + JSON.stringify(context, null, 2));
const flags = {};

const commonChanged = context.common_changes.licence_changed;

core.startGroup('Evaluate Projects');
for (const projName of Object.keys(context.projects)) {
    const p = context.projects[projName];

    const reasons = {
        licence_changed: context.common_changes.licence_changed,
        props_changed: p.changes.props_changed,
        version_changed: p.version.is_changed,
        code_changed: p.changes.code_changed,
        test_changed: p.changes.test_changed,
        nuget_changed: p.changes.nuget_changed,
        project_ref: p.changes.project_ref
    }

    const projectNeedsUpdate = Object.values(reasons).some(r => r === true);
    if (projectNeedsUpdate)
        core.info("Project " + projName + " needs update.\nReason: " + JSON.stringify(reasons, null, 2));
    flags[projName] = !!projectNeedsUpdate;
}
core.endGroup();

const flagsPath = path.join(workspace, 'version-change-flags.json');
fs.writeFileSync(flagsPath, JSON.stringify(flags, null, 2));
core.info('version-change-flags.json generated successfully.');

core.startGroup('Generate Matrix');
const matrixProjects = [];
for (const projName of Object.keys(flags)) {
    if (flags[projName]) {
        const p = context.projects[projName];
        matrixProjects.push({
            "folder": path.dirname(p.csproj_path) + "/**",
            "csproj": p.csproj_path,
            "package-id": projName,
            "version": p.version.new,
            "artifact": "nupkg-" + projName.replace(/\./g, '-').toLowerCase()
        });
    }
}
core.endGroup();

// Write to GITHUB_OUTPUT so subsequent steps can check if tests are needed
const isNeedUnittest = Object.values(flags).some(f => f === true);
core.info('isNeedUnittest: ' + isNeedUnittest);
core.info('matrixProjects: \n' + JSON.stringify(matrixProjects, null, 2));

core.setOutput('is_need_unittest', isNeedUnittest);
core.setOutput('matrix', { project: matrixProjects });

