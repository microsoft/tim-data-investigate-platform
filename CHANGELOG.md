# Changelog

## [2.1.2](https://github.com/microsoft/tim-data-investigate-platform/compare/core-v2.1.1...core-v2.1.2) (2023-07-24)


### Bug Fixes

* env inject config.js and nginx ([#27](https://github.com/microsoft/tim-data-investigate-platform/issues/27)) ([9fa9eb9](https://github.com/microsoft/tim-data-investigate-platform/commit/9fa9eb9115d15f3f0b8f6a0ce6b50517330d4e36))

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
