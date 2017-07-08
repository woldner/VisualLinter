# VisualLinter

[![Build status](https://ci.appveyor.com/api/projects/status/e34sj7pi1v3unlf0?svg=true)](https://ci.appveyor.com/project/jwldnr/visuallinter)

<!-- Update the VS Gallery link after you upload the VSIX-->
Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/[GuidFromGallery])
or get the [CI build](http://vsixgallery.com/extension/832aee43-88e1-4e51-ac31-d412d356dfdf/).

---------------------------------------

Visual Studio Linter using ESLint (installed globally) with visual feedback in the text buffer.

See the [change log](CHANGELOG.md) for changes and road map.

## Features

- Lint files using ESLint on file save.

## Requirements

- ESLint installed globally e.g. `npm i eslint@latest --global`
- A valid config (create one using `eslint --init` after installing ESLint)

Please note that additional ESLint plugins may be required in order to lint files depending on your configuration. The `VisualLinter` output window could help identify these plugins.

## Troubleshooting

- If you receive a error message when starting Visual Studio with the extension installed saying that ESLint could not be found in `PATH`, restarting your computer would reload the environment variables.
- If you receive a message saying that `eslint` could not verify your configuration, refer to the docs on [how to configure ESLint](http://eslint.org/docs/user-guide/configuring)

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

For cloning and building this project yourself, make sure
to install the
[Extensibility Tools 2015](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features
used by this project.

## License
[Apache 2.0](LICENSE)