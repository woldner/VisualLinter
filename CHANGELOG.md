# Road map

- Automatically fix errors (provided by `ESLint`)

Features that have a checkmark are complete and available for download in the
[CI build](http://vsixgallery.com/extension/832aee43-88e1-4e51-ac31-d412d356dfdf/).

# Change log

These are the changes to each version that has been released on the official Visual Studio extension gallery.

## 1.1 (2018-01-03)

#### Fix

- Poorly misplaced finally-clause causing deadlock

## 1.1 (2017-12-15)

##### Fix

- Option values persist when switching between views

## 1.1 (2017-12-10)

##### Fix

- Fix process timeout issue

##### Change

- Improve performance
- Change the way ESLint process is run
- Add [Known Issues](README.md#known-issues) section to the [README](README.md)

## 1.0 (2017-11-21)

##### Change

- Add `Show debug information` option
- Import options in `ViewModelBase`

## 1.0 (2017-10-21)

##### Change

- Add `.html` file extension support
- Add `.vue` file extension support

## 1.0 (2017-09-28)

##### Fix

- Fix bug where trailing semi markers would not update in buffer

##### Change

- Improve tagging

## 1.0 (2017-09-24)

##### Change

- Add `.jsx` file extension support

## 1.0 (2017-09-19)

##### Change

- Add `.eslintignore` support
- Add option to disable the use of `.eslintignore` when linting files

## 1.0 (2017-09-09)

##### Fix

- Fix argument out of range exception bug

## 1.0 (2017-08-31)

##### Change

- Wider search for ESLint installation/config

## 1.0 (2017-08-30)

##### Change

- Change the default behavior to use a local installation instead of global
- Add option to use global installation

## 1.0 (2017-08-16)

##### Change

- Add options page
- Change the default behavior to use a local config (relative to file being linted) instead of personal
- Add option to use personal config

## 1.0 (2017-07-08)

- Initial release