# VisualLinter

[![Build status](https://ci.appveyor.com/api/projects/status/e34sj7pi1v3unlf0?svg=true)](https://ci.appveyor.com/project/jwldnr/visuallinter)

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

- ESLint installed globally e.g. `npm i eslint@latest --global`
- A valid config located in your user's home directory (create one using `eslint --init` after installing ESLint)

##### Notes

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
[Apache 2.0](LICENSE.txt)