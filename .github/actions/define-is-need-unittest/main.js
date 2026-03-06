const needsChangeInput = process.argv[2];

if (!needsChangeInput) {
    console.error("Usage: node main.js <needs_change_json>");
    process.exit(1);
}

let needsChange;
try {
    needsChange = JSON.parse(needsChangeInput);
} catch (e) {
    console.error("Error parsing needs change JSON:", e.message);
    process.exit(1);
}

let isNeedUnittest = false;

for (const projId in needsChange) {
    if (needsChange[projId] === true) {
        isNeedUnittest = true;
        break;
    }
}

console.log("Is need unittest:", isNeedUnittest);

if (process.env.GITHUB_OUTPUT) {
    const fs = require('fs');
    fs.appendFileSync(process.env.GITHUB_OUTPUT, `is_need_unittest=${isNeedUnittest}\n`);
}
