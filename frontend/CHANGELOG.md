# Changelog

## [1.5.6](https://github.com/itsnotapt/tim-data-investigate-platform/compare/frontend-v1.5.5...frontend-v1.5.6) (2023-02-26)


### Bug Fixes

* nginx 426 error ([b2e6f12](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b2e6f12429d27d656c5b46e272dada3d2d8b451b))

## [1.5.5](https://github.com/itsnotapt/tim-data-investigate-platform/compare/frontend-v1.5.4...frontend-v1.5.5) (2023-02-26)


### Bug Fixes

* support for ssl nginx ([16e760e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/16e760e144bad257e7a5a9772d2ab7a9ad8e5464))

## [1.5.3](https://github.com/itsnotapt/tim-data-investigate-platform/compare/frontend-v1.5.2...frontend-v1.5.3) (2023-02-22)


### Bug Fixes

* remove host header from nginx ([6db99a5](https://github.com/itsnotapt/tim-data-investigate-platform/commit/6db99a53c918ff91666efec7054a83a2b7da71c7))

## [1.5.2](https://github.com/itsnotapt/tim-data-investigate-platform/compare/frontend-v1.5.1...frontend-v1.5.2) (2023-02-21)


### Bug Fixes

* config.js now blocking. ([fd4e914](https://github.com/itsnotapt/tim-data-investigate-platform/commit/fd4e9145f65930cced163f50af17c66dd5231121))

## [1.5.1](https://github.com/itsnotapt/tim-data-investigate-platform/compare/frontend-v1.5.0...frontend-v1.5.1) (2023-02-18)


### Bug Fixes

* config.js defer loading. ([53adc0c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/53adc0cb322a11ebab9ed619c5ed17eac4bdc0bc))
* config.js defer loading. ([0b4718a](https://github.com/itsnotapt/tim-data-investigate-platform/commit/0b4718a660a62378c88337593e55325be08476a5))
* nginx refer to backend. ([53adc0c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/53adc0cb322a11ebab9ed619c5ed17eac4bdc0bc))
* nginx refer to backend. ([0b4718a](https://github.com/itsnotapt/tim-data-investigate-platform/commit/0b4718a660a62378c88337593e55325be08476a5))

## [1.5.0](https://github.com/itsnotapt/tim-data-investigate-platform/compare/frontend-v1.4.0...frontend-v1.5.0) (2023-02-17)


### Features

* add syntax highlighting and autocomplete with Kusto Monaco. ([#8](https://github.com/itsnotapt/tim-data-investigate-platform/issues/8)) ([1326939](https://github.com/itsnotapt/tim-data-investigate-platform/commit/1326939629a66aeff37a60a5d748686d79ecd94d))
* reintroduce AAD authentication and OBO. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* support for schema autocomplete for queries. ([#18](https://github.com/itsnotapt/tim-data-investigate-platform/issues/18)) ([b5a9121](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))


### Bug Fixes

* add runtime config to frontend. ([#10](https://github.com/itsnotapt/tim-data-investigate-platform/issues/10)) ([a0a7070](https://github.com/itsnotapt/tim-data-investigate-platform/commit/a0a707023445c8a826aaa5453fbfb43b8f2a1122))
* add time to live to couchbase upsert. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* always prompt to select account on logon. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* env inject config.js and nginx ([#27](https://github.com/itsnotapt/tim-data-investigate-platform/issues/27)) ([9fa9eb9](https://github.com/itsnotapt/tim-data-investigate-platform/commit/9fa9eb9115d15f3f0b8f6a0ce6b50517330d4e36))
* execution status panel wrong props. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* fake fix to initiate rebuild. ([02637f0](https://github.com/itsnotapt/tim-data-investigate-platform/commit/02637f0cd3d4361d4a6b8e75f6b36870c89598b4))
* load monaco kusto once. ([b5a9121](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* match backend API changes. ([#14](https://github.com/itsnotapt/tim-data-investigate-platform/issues/14)) ([e1c68a4](https://github.com/itsnotapt/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* move cluster groups into runtime config. ([e1c68a4](https://github.com/itsnotapt/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* split kusto monaco and others. ([b5a9121](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* support for building aggrid enterprise. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* wrong format for kusto cluster group. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))

## [1.4.0](https://github.com/microsoft/tim-data-investigate-platform/compare/frontend-v1.3.2...frontend-v1.4.0) (2023-02-15)


### Features

* support for schema autocomplete for queries. ([#18](https://github.com/microsoft/tim-data-investigate-platform/issues/18)) ([b5a9121](https://github.com/microsoft/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))


### Bug Fixes

* load monaco kusto once. ([b5a9121](https://github.com/microsoft/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* split kusto monaco and others. ([b5a9121](https://github.com/microsoft/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))

## [1.3.2](https://github.com/microsoft/tim-data-investigate-platform/compare/frontend-v1.3.1...frontend-v1.3.2) (2023-02-14)


### Bug Fixes

* match backend API changes. ([#14](https://github.com/microsoft/tim-data-investigate-platform/issues/14)) ([e1c68a4](https://github.com/microsoft/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* move cluster groups into runtime config. ([e1c68a4](https://github.com/microsoft/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))

## [1.3.1](https://github.com/microsoft/tim-data-investigate-platform/compare/frontend-v1.3.0...frontend-v1.3.1) (2023-02-13)


### Bug Fixes

* fake fix to initiate rebuild. ([02637f0](https://github.com/microsoft/tim-data-investigate-platform/commit/02637f0cd3d4361d4a6b8e75f6b36870c89598b4))

## [1.3.0](https://github.com/microsoft/tim-data-investigate-platform/compare/frontend-v1.2.0...frontend-v1.3.0) (2023-02-13)


### Features

* add syntax highlighting and autocomplete with Kusto Monaco. ([#8](https://github.com/microsoft/tim-data-investigate-platform/issues/8)) ([1326939](https://github.com/microsoft/tim-data-investigate-platform/commit/1326939629a66aeff37a60a5d748686d79ecd94d))


### Bug Fixes

* add runtime config to frontend. ([#10](https://github.com/microsoft/tim-data-investigate-platform/issues/10)) ([a0a7070](https://github.com/microsoft/tim-data-investigate-platform/commit/a0a707023445c8a826aaa5453fbfb43b8f2a1122))

## [1.2.0](https://github.com/microsoft/tim-data-investigate-platform/compare/frontend-v1.1.0...frontend-v1.2.0) (2023-02-10)


### Bug Fixes

* always prompt to select account on logon. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* execution status panel wrong props. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* support for building aggrid enterprise. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
