# Changelog

## [3.0.7](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.6...core-v3.0.7) (2023-03-04)


### Bug Fixes

* starttime and endtime for kusto queries ([c9ce5f3](https://github.com/itsnotapt/tim-data-investigate-platform/commit/c9ce5f3d3889c185553effccf7b7314c91ea1190))

## [3.0.6](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.5...core-v3.0.6) (2023-02-26)


### Bug Fixes

* nginx 426 error ([b2e6f12](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b2e6f12429d27d656c5b46e272dada3d2d8b451b))

## [3.0.5](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.4...core-v3.0.5) (2023-02-26)


### Bug Fixes

* support for ssl nginx ([16e760e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/16e760e144bad257e7a5a9772d2ab7a9ad8e5464))

## [3.0.4](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.3...core-v3.0.4) (2023-02-26)


### Bug Fixes

* capture exception for kusto and throw database error. ([02ad822](https://github.com/itsnotapt/tim-data-investigate-platform/commit/02ad822a14e1a6cd75b87065061302672da46004))

## [3.0.3](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.2...core-v3.0.3) (2023-02-22)


### Bug Fixes

* remove host header from nginx ([6db99a5](https://github.com/itsnotapt/tim-data-investigate-platform/commit/6db99a53c918ff91666efec7054a83a2b7da71c7))

## [3.0.2](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.1...core-v3.0.2) (2023-02-21)


### Bug Fixes

* config.js now blocking. ([fd4e914](https://github.com/itsnotapt/tim-data-investigate-platform/commit/fd4e9145f65930cced163f50af17c66dd5231121))
* slight adjustment to env variables. ([3de24ac](https://github.com/itsnotapt/tim-data-investigate-platform/commit/3de24ac2b9477443a23e651f63946ead2743650d))

## [3.0.1](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v3.0.0...core-v3.0.1) (2023-02-18)


### Bug Fixes

* config.js defer loading. ([53adc0c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/53adc0cb322a11ebab9ed619c5ed17eac4bdc0bc))
* config.js defer loading. ([0b4718a](https://github.com/itsnotapt/tim-data-investigate-platform/commit/0b4718a660a62378c88337593e55325be08476a5))
* nginx refer to backend. ([53adc0c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/53adc0cb322a11ebab9ed619c5ed17eac4bdc0bc))
* nginx refer to backend. ([0b4718a](https://github.com/itsnotapt/tim-data-investigate-platform/commit/0b4718a660a62378c88337593e55325be08476a5))

## [3.0.0](https://github.com/itsnotapt/tim-data-investigate-platform/compare/core-v2.1.1...core-v3.0.0) (2023-02-17)


### ⚠ BREAKING CHANGES

* add get kusto schema.

### Features

* add get kusto schema. ([8ba47f7](https://github.com/itsnotapt/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))
* add support for mongodb and redis ([#22](https://github.com/itsnotapt/tim-data-investigate-platform/issues/22)) ([95bcf2e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* add syntax highlighting and autocomplete with Kusto Monaco. ([#8](https://github.com/itsnotapt/tim-data-investigate-platform/issues/8)) ([1326939](https://github.com/itsnotapt/tim-data-investigate-platform/commit/1326939629a66aeff37a60a5d748686d79ecd94d))
* reintroduce AAD authentication and OBO. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* support for schema autocomplete for queries. ([#18](https://github.com/itsnotapt/tim-data-investigate-platform/issues/18)) ([b5a9121](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))


### Bug Fixes

* add runtime config to frontend. ([#10](https://github.com/itsnotapt/tim-data-investigate-platform/issues/10)) ([a0a7070](https://github.com/itsnotapt/tim-data-investigate-platform/commit/a0a707023445c8a826aaa5453fbfb43b8f2a1122))
* add time to live to couchbase upsert. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* always prompt to select account on logon. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* continue even if Kusto create tables failed. ([1ac713f](https://github.com/itsnotapt/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))
* enum conversion on query run status. ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* env inject config.js and nginx ([#27](https://github.com/itsnotapt/tim-data-investigate-platform/issues/27)) ([9fa9eb9](https://github.com/itsnotapt/tim-data-investigate-platform/commit/9fa9eb9115d15f3f0b8f6a0ce6b50517330d4e36))
* execution status panel wrong props. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* fake fix to initiate rebuild. ([02637f0](https://github.com/itsnotapt/tim-data-investigate-platform/commit/02637f0cd3d4361d4a6b8e75f6b36870c89598b4))
* increase timeout for initializing couchbase db. ([#15](https://github.com/itsnotapt/tim-data-investigate-platform/issues/15)) ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* load monaco kusto once. ([b5a9121](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* match backend API changes. ([#14](https://github.com/itsnotapt/tim-data-investigate-platform/issues/14)) ([e1c68a4](https://github.com/itsnotapt/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* move cluster groups into runtime config. ([e1c68a4](https://github.com/itsnotapt/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* only load the selected database. ([95bcf2e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* prevent camelcase default for couchbase. ([95bcf2e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* proper model validation instead of 500 exceptions. ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* return correct status for execute query and get results. ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* split kusto monaco and others. ([b5a9121](https://github.com/itsnotapt/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* support for building aggrid enterprise. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* support for cosmodb ([#24](https://github.com/itsnotapt/tim-data-investigate-platform/issues/24)) ([c58caf6](https://github.com/itsnotapt/tim-data-investigate-platform/commit/c58caf6713d3b61f329efc3cf297ca51bced3c88))
* use default auth for azure to support managed identities. ([#20](https://github.com/itsnotapt/tim-data-investigate-platform/issues/20)) ([1ac713f](https://github.com/itsnotapt/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))
* validation of cluster uri with https. ([#17](https://github.com/itsnotapt/tim-data-investigate-platform/issues/17)) ([8ba47f7](https://github.com/itsnotapt/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))
* wrong format for kusto cluster group. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))

## [2.1.1](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v2.1.0...core-v2.1.1) (2023-02-17)


### Bug Fixes

* support for cosmodb ([#24](https://github.com/microsoft/tim-data-investigate-platform/issues/24)) ([c58caf6](https://github.com/microsoft/tim-data-investigate-platform/commit/c58caf6713d3b61f329efc3cf297ca51bced3c88))

## [2.1.0](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v2.0.1...core-v2.1.0) (2023-02-17)


### Features

* add support for mongodb and redis ([#22](https://github.com/microsoft/tim-data-investigate-platform/issues/22)) ([95bcf2e](https://github.com/microsoft/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))


### Bug Fixes

* only load the selected database. ([95bcf2e](https://github.com/microsoft/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* prevent camelcase default for couchbase. ([95bcf2e](https://github.com/microsoft/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))

## [2.0.1](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v2.0.0...core-v2.0.1) (2023-02-16)


### Bug Fixes

* continue even if Kusto create tables failed. ([1ac713f](https://github.com/microsoft/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))
* use default auth for azure to support managed identities. ([#20](https://github.com/microsoft/tim-data-investigate-platform/issues/20)) ([1ac713f](https://github.com/microsoft/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))

## [2.0.0](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v1.3.2...core-v2.0.0) (2023-02-15)


### ⚠ BREAKING CHANGES

* modified the API for kusto queries.

### Features

* add get kusto schema. ([8ba47f7](https://github.com/microsoft/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))
* support for schema autocomplete for queries. ([#18](https://github.com/microsoft/tim-data-investigate-platform/issues/18)) ([b5a9121](https://github.com/microsoft/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))


### Bug Fixes

* load monaco kusto once. ([b5a9121](https://github.com/microsoft/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* split kusto monaco and others. ([b5a9121](https://github.com/microsoft/tim-data-investigate-platform/commit/b5a9121f98bb33112726cb93d7d8f848ec02fb9c))
* validation of cluster uri with https. ([#17](https://github.com/microsoft/tim-data-investigate-platform/issues/17)) ([8ba47f7](https://github.com/microsoft/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))

## [1.3.2](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v1.3.1...core-v1.3.2) (2023-02-14)


### Bug Fixes

* enum conversion on query run status. ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* increase timeout for initializing couchbase db. ([#15](https://github.com/microsoft/tim-data-investigate-platform/issues/15)) ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* match backend API changes. ([#14](https://github.com/microsoft/tim-data-investigate-platform/issues/14)) ([e1c68a4](https://github.com/microsoft/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* move cluster groups into runtime config. ([e1c68a4](https://github.com/microsoft/tim-data-investigate-platform/commit/e1c68a4a0e3e0c221d601325e185bf0895131fc1))
* proper model validation instead of 500 exceptions. ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* return correct status for execute query and get results. ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))

## [1.3.1](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v1.3.0...core-v1.3.1) (2023-02-13)


### Bug Fixes

* fake fix to initiate rebuild. ([02637f0](https://github.com/microsoft/tim-data-investigate-platform/commit/02637f0cd3d4361d4a6b8e75f6b36870c89598b4))

## [1.3.0](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v1.2.0...core-v1.3.0) (2023-02-13)


### Features

* add syntax highlighting and autocomplete with Kusto Monaco. ([#8](https://github.com/microsoft/tim-data-investigate-platform/issues/8)) ([1326939](https://github.com/microsoft/tim-data-investigate-platform/commit/1326939629a66aeff37a60a5d748686d79ecd94d))


### Bug Fixes

* add runtime config to frontend. ([#10](https://github.com/microsoft/tim-data-investigate-platform/issues/10)) ([a0a7070](https://github.com/microsoft/tim-data-investigate-platform/commit/a0a707023445c8a826aaa5453fbfb43b8f2a1122))

## [1.2.0](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v1.1.1...core-v1.2.0) (2023-02-10)


### Features

* reintroduce AAD authentication and OBO. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))


### Bug Fixes

* add time to live to couchbase upsert. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* always prompt to select account on logon. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* execution status panel wrong props. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* support for building aggrid enterprise. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* wrong format for kusto cluster group. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
