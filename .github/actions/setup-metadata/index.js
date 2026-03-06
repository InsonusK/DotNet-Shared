const core = require('@actions/core');
const github = require('@actions/github');
const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

function getAllFiles(dirPath, arrayOfFiles) {
    const files = fs.readdirSync(dirPath);
    arrayOfFiles = arrayOfFiles || [];
    files.forEach((file) => {
        if (fs.statSync(dirPath + "/" + file).isDirectory()) {
            // Игнорируем bin, obj и .git
            if (!file.match(/^(bin|obj|\.git)$/)) {
                arrayOfFiles = getAllFiles(dirPath + "/" + file, arrayOfFiles);
            }
        } else if (file.endsWith('.csproj')) {
            arrayOfFiles.push(path.join(dirPath, file));
        }
    });
    return arrayOfFiles;
};

function loadCentralPackages(packagesPropsPath) {
    const centralPackages = {};
    if (fs.existsSync(packagesPropsPath)) {
        const propsContent = fs.readFileSync(packagesPropsPath, 'utf8');
        const packageRegex = /<PackageVersion\s+Include="([^"]+)"\s+Version="([^"]+)"/g;
        let match;
        while ((match = packageRegex.exec(propsContent)) !== null) {
            centralPackages[match[1]] = match[2];
        }
    } else {
        throw new Error(`Файл Directory.Packages.props не найден по пути: ${packagesPropsPath}. В монорепозитории централизованное управление версиями пакетов обязательно.`);
    }
    return centralPackages;
}

function getCSPROJrelations(csprojFiles, workspace, changedFiles) {
    const projectsData = {};

    csprojFiles.forEach(projectPath => {
        const content = fs.readFileSync(projectPath, 'utf8');
        const projectName = path.basename(projectPath, '.csproj');
        const projectDir = path.dirname(projectPath).replace(workspace + '/', '');

        // Ищем зависимости NuGet
        const nugetRegex = /<PackageReference\s+Include="([^"]+)"/g;
        const nugets = [];
        let match;
        while ((match = nugetRegex.exec(content)) !== null) {
            nugets.push(match[1]);
        }

        // Ищем зависимости ProjectReference
        const projRefRegex = /<ProjectReference\s+Include="([^"]+)"/g;
        const projectRefs = [];
        while ((match = projRefRegex.exec(content)) !== null) {
            // Получаем имя связанного проекта из пути
            const refName = path.basename(match[1], '.csproj');
            projectRefs.push(refName);
        }

        // Проверяем, менялись ли файлы в папке этого проекта
        const isChanged = changedFiles.some(f => f.startsWith(projectDir));

        projectsData[projectName] = {
            path: projectPath.replace(workspace + '/', ''), // путь до проекта
            directory: projectDir, // директория
            nugetDependencies: nugets, // используемые пакеты
            projectDependencies: projectRefs, // используемые проекты
            hasLocalChanges: isChanged // директория изменились
        };
    });
    return projectsData;
}

async function run() {
    try {
        const token = core.getInput('github-token');
        const baseBranch = core.getInput('base-branch');
        const workspace = process.env.GITHUB_WORKSPACE;

        if (!workspace) {
            throw new Error('Переменная GITHUB_WORKSPACE не определена. Запускается ли скрипт в среде GitHub Actions?');
        }

        core.info('1. Получение списка измененных файлов...');
        // Используем git diff. ВАЖНО: в шаге checkout нужно использовать fetch-depth: 0
        let changedFiles = [];
        try {
            const diffCmd = `git diff --name-only ${baseBranch} HEAD`;
            changedFiles = execSync(diffCmd, { cwd: workspace }).toString().split('\n').filter(Boolean);
        } catch (e) {
            throw new Error(`Критическая ошибка при вычислении git diff. Убедитесь, что в шаге checkout используется 'fetch-depth: 0'. Подробности: ${e.message}`);
        }

        core.info('2. Поиск всех .csproj файлов...');
        const csprojFiles = getAllFiles(workspace);
        core.info("Найдены .csproj файлы:" + JSON.stringify(csprojFiles))

        core.info('3. Парсинг Directory.Packages.props...');
        const packagesPropsPath = path.join(workspace, 'Directory.Packages.props');
        const centralPackages = loadCentralPackages(packagesPropsPath);
        (let packagesPropsChanged, let changedPackageNames) = extractChangedPackages(changedFiles, baseBranch, workspace);
        core.info("Найдено пакетов из Directory.Packages.props: " + JSON.stringify(centralPackages))

        core.info('4. Анализ связей проектов...');
        const projectsData = getCSPROJrelations(csprojFiles, workspace, changedFiles);
        core.info("Собрана информация о проектах: " + JSON.stringify(projectsData))

        core.info('5. Формирование контекста...');
        const ciContext = {
            global: {
                packagesPropsChanged: packagesPropsChanged,
                centralPackageVersions: centralPackages, // Пакеты в Directory.Packages.props и их версии
                allChangedFiles: changedFiles // Список измененных файлов
            },
            projects: projectsData // Информация о проектах
        };

        const contextFilePath = path.join(workspace, 'ci-context.json');
        fs.writeFileSync(contextFilePath, JSON.stringify(ciContext, null, 2));

        // Передаем путь к файлу как output
        core.setOutput('context-path', contextFilePath);
        core.info(`Метаданные успешно сохранены в ${contextFilePath}`);

    } catch (error) {
        core.setFailed(`Ошибка выполнения Action: ${error.message}`);
    }
}

run();


function extractChangedPackages(changedFiles, baseBranch, workspace) {
    const changedPackageNames = new Set();
    let packagesPropsChanged = changedFiles.some(f => f.includes('Directory.Packages.props'));

    // Если файл менялся, запрашиваем точечный diff только по нему
    if (packagesPropsChanged) {
        try {
            // -U0 убирает контекстные строки, оставляя только реально измененные
            const diffCmd = `git diff -U0 ${baseBranch} HEAD -- Directory.Packages.props`;
            const diffOutput = execSync(diffCmd, { cwd: workspace, stdio: 'pipe' }).toString();

            // Ищем строки, начинающиеся с + или - (добавленные/удаленные/измененные)
            // Игнорируем строки +++ и --- (заголовки git diff)
            const diffLines = diffOutput.split('\n').filter(line => /^[+-][^+-]/.test(line));
            const diffRegex = /<PackageVersion\s+Include="([^"]+)"/;

            diffLines.forEach(line => {
                const match = diffRegex.exec(line);
                if (match) {
                    changedPackageNames.add(match[1]);
                }
            });
            core.info(`Изменены версии пакетов: ${Array.from(changedPackageNames).join(', ') || 'нет (возможно, форматирование)'}`);
        } catch (e) {
            throw new Error(`Ошибка при анализе diff файла Directory.Packages.props: ${e.message}`);
        }
    }
    return (packagesPropsChanged, changedPackageNames);
}

