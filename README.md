# VisualLinter

[![Build status](https://ci.appveyor.com/api/projects/status/9ihx1afw1cc1e9b4?svg=true)](https://ci.appveyor.com/project/jwldnr/visuallinter)

Visual Studio JavaScript linter using ESLint.

Download this extension from the [VS Gallery](https://marketplace.visualstudio.com/vsgallery/a71a5b0d-9f75-4cd2-b1f1-c4afb79a0638)
or get the [CI build](http://vsixgallery.com/extension/21d9f99b-ec42-4df4-8b16-2a62db5392a5/).

---------------------------------------

See the [change log](CHANGELOG.md) for changes and road map.

## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
- [Language Support](#language-support)
- [Troubleshooting](#troubleshooting)
- [Contribute](#contribute)
- [Thanks to](#thanks-to)

## Features

- Lint files using ESLint on file save. (see [Language Support](#language-support))

## Getting Started

##### Requirements

- ESLint installed locally (per project) _or_ globally (added to PATH variable)
- A valid ESLint configuration located in the current project folder (local) _or_ your user's home directory (global)

##### Notes

`VisualLinter` will always use the closest configuration file found relative to the file being linted.
If no `.eslintrc` file is found in the same directory as the file being linted, VisualLinter will try and find a configuration file in ancestor directories up to, but not including the root directory (e.g. `C:\\` on Windows).

For instance, suppose you have the following structure:

```
your-project
├── .eslintrc
├── lib
│ └── source.js
└─┬ tests
  ├── .eslintrc
  └── test.js
```

When you open or save `source.js` `VisualLinter` will use the `.eslintrc` file at the root of the project as its configuration.

When you open or save `test.js` `VisualLinter` will use the `.eslintrc` file in the `tests/` directory.

If you have a personal configuration file in your home directory (`~/.eslintrc`), it will only be used if the checkbox `'Use personal ESLint config instead of local'` is checked under `Tools > Options >  VisualLinter`.

If there are multiple configuration files in the same directory, `VisualLinter` will only use one. The priority order is:

1. .eslintrc.js
2. .eslintrc.yaml
3. .eslintrc.yml
4. .eslintrc.json
5. .eslintrc

Please note that additional ESLint plugins may be required in order to lint files depending on your configuration. The `VisualLinter` output window could help identify these plugins.

## Language Support

| Language | File Extensions | Supported Linters |
| --- | --- | ---- |
| JavaScript | `.js` | [`ESLint`](https://github.com/eslint/eslint) |

## Troubleshooting

- If you receive a error message when starting Visual Studio with the extension installed saying that ESLint could not be found in `PATH`, restarting your computer would reload the environment variables.
- If you receive a message saying that `eslint` could not verify your configuration, refer to the docs on [how to configure ESLint](http://eslint.org/docs/user-guide/configuring)
- Visual Studio 2017 is using ESLint by default, however it does not show errors in buffer, if you're seeing duplicate messages in the error list window, you can disable the built in linter by setting `Enable ESLint` to `false` in:

  `Options` > `Text Editor` > `JavaScript/TypeScript` > `ESLint`

## Contribute
[See all contributors on GitHub.](https://github.com/jwldnr/VisualLinter/graphs/contributors)

Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

For cloning and building this project yourself, make sure
to install the
[Extensibility Tools 2015](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features
used by this project.

## Thanks to

Creators of..

- [SonarSource/SonarLint](https://github.com/SonarSource/sonarlint-visualstudio) extension for Visual Studio.
- [AtomLinter/linter-eslint](https://github.com/AtomLinter/linter-eslint/) eslint package for the Atom text editor.

## License
[MIT](LICENSE)