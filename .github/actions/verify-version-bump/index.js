const fs = require('fs');
const path = require('path');
const core = require('@actions/core');

const workspace = process.env.GITHUB_WORKSPACE || process.cwd();
const contextPath = path.join(workspace, 'ci-context.json');
const flagsPath = path.join(workspace, 'version-change-flags.json');

if (!fs.existsSync(contextPath) || !fs.existsSync(flagsPath)) {
    core.setFailed("Required JSON files missing.");
    process.exit(1);
}

const context = JSON.parse(fs.readFileSync(contextPath, 'utf8'));
core.info('context: \n' + JSON.stringify(context, null, 2));
const flags = JSON.parse(fs.readFileSync(flagsPath, 'utf8'));
core.info('flags: \n' + JSON.stringify(flags, null, 2));

core.startGroup('Verify Version Bumps');
let failed = false;

for (const projName of Object.keys(flags)) {
    if (flags[projName] === true) {
        const p = context.projects[projName];
        if (!p.version.is_changed) {
            core.error(`Project ${projName} has changes but its version was not bumped!`);
            failed = true;
        } else {
            core.info(`Project ${projName}: Version correctly bumped from ${p.version.prev} to ${p.version.new}.`);
        }
    } else {
        core.info(`Project ${projName}: No functional changes detected, version bump not required.`);
    }
}
core.endGroup();

if (failed) {
    core.setFailed("Version check failed.");
    process.exit(1);
} else {
    core.info("All required versions have been bumped.");
}
