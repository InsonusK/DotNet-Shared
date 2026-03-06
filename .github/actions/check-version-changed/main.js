const fs = require('fs');
const { execSync } = require('child_process');
const path = require('path');

const needsChangeInput = process.argv[2];
const projectsPath = process.argv[3];
const baseRef = process.argv[4];

if (!needsChangeInput || !projectsPath || !baseRef) {
    console.error("Usage: node main.js <needs_change_json> <projects.json> <base_ref>");
    process.exit(1);
}

let needsChangeInputObject;
try {
    needsChangeInputObject = JSON.parse(needsChangeInput);
    console.log("Needs Change JSON:", needsChangeInputObject);
} catch (e) {
    console.error("Error parsing needs change JSON:", e.message);
    process.exit(1);
}

let projectsConfig;
try {
    projectsConfig = JSON.parse(fs.readFileSync(projectsPath, 'utf8'));
    console.log("Projects JSON:", projectsConfig);
} catch (e) {
    console.error("Error reading projects.json:", e.message);
    process.exit(1);
}

function getPackageVersion(content) {
    const match = content.match(/<PackageVersion>([^<]+)<\/PackageVersion>/i);
    return match ? match[1].trim() : null;
}

const results = {};

for (const proj of projectsConfig.project) {
    const projId = proj["package-id"] || proj.folder;
    console.log("Project ID:", projId);

    // Check if the project actually exists locally
    if (!proj.csproj || !fs.existsSync(proj.csproj)) {
        console.warn(`Could not find csproj for ${projId} with path ${proj.csproj}`);
        continue;
    }

    const mustChanged = needsChangeInputObject[projId] === true;
    console.log(`Must changed for ${projId}:`, mustChanged);
    let hasChanged = false;

    try {
        const localContent = fs.readFileSync(proj.csproj, 'utf8');
        const localVersion = getPackageVersion(localContent);
        console.log("Local version:", localVersion);

        // We need the relative path from git root to properly use `git show`
        const csprojRelativePath = path.relative(path.resolve('.'), path.resolve(proj.csproj)).replace(/\\/g, '/');

        // Suppress stderr to avoid spamming if file didn't exist in base ref
        const baseContent = execSync(`git show ${baseRef}:${csprojRelativePath}`, { encoding: 'utf8', stdio: ['pipe', 'pipe', 'ignore'] });
        const baseVersion = getPackageVersion(baseContent);
        console.log("Base version:", baseVersion);

        if (localVersion && baseVersion && localVersion !== baseVersion) {
            hasChanged = true;
            console.log(`Project ${projId}: Version changed from ${baseVersion} to ${localVersion}`);
        } else if (!baseVersion && localVersion) {
            // New project
            hasChanged = true;
            console.log(`Project ${projId}: New project`);
        }
        console.log(`Project ${projId}: hasChanged = ${hasChanged}`);
    } catch (e) {
        // Ignored. Probably file didn't exist in base ref.
        // We can consider that the version has "changed" (it was introduced) if we have it locally.
        hasChanged = true;
        console.log(`Failed to read base version for ${projId}. Assuming new file.`);
    }

    results[projId] = {
        mustChanged: mustChanged,
        hasChanged: hasChanged
    };
    console.log("Result:", results[projId]);
}

const jsonResult = JSON.stringify(results);
console.log("JSON Result:", jsonResult);

if (process.env.GITHUB_OUTPUT) {
    fs.appendFileSync(process.env.GITHUB_OUTPUT, `versions=${jsonResult}\n`);
}

let failedProjects = [];
for (const projId in results) {
    if (results[projId].mustChanged && !results[projId].hasChanged) {
        failedProjects.push(projId);
    }
}

if (failedProjects.length > 0) {
    console.error(`\n::error::The following projects MUST have their version changed, but their version remained the same: ${failedProjects.join(', ')}`);
    process.exit(1);
} else {
    console.log("\n✅ All project versions correctly updated.");
}
