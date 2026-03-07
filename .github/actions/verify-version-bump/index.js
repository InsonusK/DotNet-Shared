const fs = require('fs');
const path = require('path');

const workspace = process.env.GITHUB_WORKSPACE || process.cwd();
const contextPath = path.join(workspace, 'ci-context.json');
const flagsPath = path.join(workspace, 'version-change-flags.json');

if (!fs.existsSync(contextPath) || !fs.existsSync(flagsPath)) {
    console.error("Required JSON files missing.");
    process.exit(1);
}

const context = JSON.parse(fs.readFileSync(contextPath, 'utf8'));
console.log('context', JSON.stringify(context, null, 2));
const flags = JSON.parse(fs.readFileSync(flagsPath, 'utf8'));
console.log('flags', JSON.stringify(flags, null, 2));

let failed = false;

for (const projName of Object.keys(flags)) {
    if (flags[projName] === true) {
        const p = context.projects[projName];
        if (!p.version.is_changed) {
            console.error(`Project ${projName} has changes but its version was not bumped!`);
            failed = true;
        } else {
            console.log(`Project ${projName}: Version correctly bumped from ${p.version.prev} to ${p.version.new}.`);
        }
    } else {
        console.log(`Project ${projName}: No functional changes detected, version bump not required.`);
    }
}

if (failed) {
    console.error("Version check failed.");
    process.exit(1);
} else {
    console.log("All required versions have been bumped.");
}
