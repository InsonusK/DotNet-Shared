const fs = require('fs');

const changesInput = process.argv[2];
const projectsPath = process.argv[3];

if (!changesInput || !projectsPath) {
    console.error("Usage: node main.js <changes_json> <projects.json>");
    process.exit(1);
}

let changes;
try {
    changes = JSON.parse(changesInput);
} catch (e) {
    console.error("Error parsing changes JSON:", e.message);
    process.exit(1);
}

let projectsConfig;
try {
    projectsConfig = JSON.parse(fs.readFileSync(projectsPath, 'utf8'));
} catch (e) {
    console.error("Error reading projects.json:", e.message);
    process.exit(1);
}

let needsChange = {};

console.log("--- Initial Evaluation ---");
projectsConfig.project.forEach(proj => {
    const projId = proj["package-id"] || proj.folder;
    const c = changes[projId] || {};
    needsChange[projId] = c.folder_changed || c.nuget_libs_changed || c.build_props_changed || c.license_changed || c.readme_changed;
    console.log(`Project ${projId} base need change: ${!!needsChange[projId]}`);
});

console.log("\n--- Transitive Dependency Evaluation ---");
let changedInLoop = true;
while (changedInLoop) {
    changedInLoop = false;
    for (const proj of projectsConfig.project) {
        const projId = proj["package-id"] || proj.folder;
        if (needsChange[projId]) continue; // Already marked for change

        if (!proj.csproj || !fs.existsSync(proj.csproj)) continue;

        const csprojContent = fs.readFileSync(proj.csproj, 'utf8');

        // Check if it references any other project that needs a change
        for (const otherProj of projectsConfig.project) {
            const otherProjId = otherProj["package-id"] || otherProj.folder;
            if (projId === otherProjId) continue;

            if (needsChange[otherProjId]) {
                const packageId = otherProj["package-id"];
                const csprojName = otherProj.csproj ? otherProj.csproj.split('/').pop() : null;

                let referencesOther = false;
                // Match PackageReference or ProjectReference
                if (packageId && csprojContent.includes(`"${packageId}"`)) referencesOther = true;
                if (packageId && csprojContent.includes(`>${packageId}<`)) referencesOther = true;
                if (csprojName && csprojContent.includes(csprojName)) referencesOther = true;

                if (referencesOther) {
                    needsChange[projId] = true;
                    changedInLoop = true;
                    console.log(`Project ${projId} implicitly needs change because it references ${otherProjId}`);
                    break;
                }
            }
        }
    }
}

const jsonResult = JSON.stringify(needsChange);
console.log("\nFinal Needs Change JSON:", jsonResult);

if (process.env.GITHUB_OUTPUT) {
    fs.appendFileSync(process.env.GITHUB_OUTPUT, `needs_change=${jsonResult}\n`);
}
