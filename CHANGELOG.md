# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## 1.3.0

- Made quick fixes into a proper scoped action.<sup>1</sup> Using the correct SDK feature has several benefits:
    - The remove in file/project/etc. options now appear in a submenu.
    - Additional scopes 'in folder' and 'in solution'.<sup>2</sup>
    - The IDE now provides better progress indicators.
    - Less code, so more robust and maintainable.

<sup>1</sup> This is extremely poorly documented - the documentation currently says to use a function that seems to have been
deprecated in 2014 and the only reference to this (and the replacements for it) is a 9-year-old reply on a Google Groups question. As such, I have no
idea if the way I'm now doing it is still correct and this update may yet need changing.

<sup>2</sup> **Warning:** replacing in solution could be very slow for large projects.

## 1.2.0

- Improved renaming - the quick action will now correctly rename all types of declarations
- Improved detection - now detect private properties with hungarian notation (eg. `_plstExample`)

## 1.1.0

- Added more type prefixes

## 1.0.2

- Initial version
