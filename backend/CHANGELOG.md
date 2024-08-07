# Changelog

## [3.0.4](https://github.com/itsnotapt/tim-data-investigate-platform/compare/backend-v3.0.3...backend-v3.0.4) (2023-03-04)


### Bug Fixes

* starttime and endtime for kusto queries ([c9ce5f3](https://github.com/itsnotapt/tim-data-investigate-platform/commit/c9ce5f3d3889c185553effccf7b7314c91ea1190))

## [3.0.2](https://github.com/itsnotapt/tim-data-investigate-platform/compare/backend-v3.0.1...backend-v3.0.2) (2023-02-26)


### Bug Fixes

* capture exception for kusto and throw database error. ([02ad822](https://github.com/itsnotapt/tim-data-investigate-platform/commit/02ad822a14e1a6cd75b87065061302672da46004))

## [3.0.1](https://github.com/itsnotapt/tim-data-investigate-platform/compare/backend-v3.0.0...backend-v3.0.1) (2023-02-21)


### Bug Fixes

* slight adjustment to env variables. ([3de24ac](https://github.com/itsnotapt/tim-data-investigate-platform/commit/3de24ac2b9477443a23e651f63946ead2743650d))

## [3.0.0](https://github.com/itsnotapt/tim-data-investigate-platform/compare/backend-v2.1.1...backend-v3.0.0) (2023-02-17)


### ⚠ BREAKING CHANGES

* add get kusto schema.

### Features

* add get kusto schema. ([8ba47f7](https://github.com/itsnotapt/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))
* add support for mongodb and redis ([#22](https://github.com/itsnotapt/tim-data-investigate-platform/issues/22)) ([95bcf2e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* reintroduce AAD authentication and OBO. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))


### Bug Fixes

* add time to live to couchbase upsert. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* always prompt to select account on logon. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* continue even if Kusto create tables failed. ([1ac713f](https://github.com/itsnotapt/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))
* enum conversion on query run status. ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* execution status panel wrong props. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* fake fix to initiate rebuild. ([02637f0](https://github.com/itsnotapt/tim-data-investigate-platform/commit/02637f0cd3d4361d4a6b8e75f6b36870c89598b4))
* increase timeout for initializing couchbase db. ([#15](https://github.com/itsnotapt/tim-data-investigate-platform/issues/15)) ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* only load the selected database. ([95bcf2e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* prevent camelcase default for couchbase. ([95bcf2e](https://github.com/itsnotapt/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* proper model validation instead of 500 exceptions. ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* return correct status for execute query and get results. ([adbc273](https://github.com/itsnotapt/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* support for building aggrid enterprise. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
* support for cosmodb ([#24](https://github.com/itsnotapt/tim-data-investigate-platform/issues/24)) ([c58caf6](https://github.com/itsnotapt/tim-data-investigate-platform/commit/c58caf6713d3b61f329efc3cf297ca51bced3c88))
* use default auth for azure to support managed identities. ([#20](https://github.com/itsnotapt/tim-data-investigate-platform/issues/20)) ([1ac713f](https://github.com/itsnotapt/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))
* validation of cluster uri with https. ([#17](https://github.com/itsnotapt/tim-data-investigate-platform/issues/17)) ([8ba47f7](https://github.com/itsnotapt/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))
* wrong format for kusto cluster group. ([887490c](https://github.com/itsnotapt/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))

## [2.1.1](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v2.1.0...backend-v2.1.1) (2023-02-17)


### Bug Fixes

* support for cosmodb ([#24](https://github.com/microsoft/tim-data-investigate-platform/issues/24)) ([c58caf6](https://github.com/microsoft/tim-data-investigate-platform/commit/c58caf6713d3b61f329efc3cf297ca51bced3c88))

## [2.1.0](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v2.0.1...backend-v2.1.0) (2023-02-17)


### Features

* add support for mongodb and redis ([#22](https://github.com/microsoft/tim-data-investigate-platform/issues/22)) ([95bcf2e](https://github.com/microsoft/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))


### Bug Fixes

* only load the selected database. ([95bcf2e](https://github.com/microsoft/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))
* prevent camelcase default for couchbase. ([95bcf2e](https://github.com/microsoft/tim-data-investigate-platform/commit/95bcf2e02758c24c8470ab89e0e82dfaeb68ad60))

## [2.0.1](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v2.0.0...backend-v2.0.1) (2023-02-16)


### Bug Fixes

* continue even if Kusto create tables failed. ([1ac713f](https://github.com/microsoft/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))
* use default auth for azure to support managed identities. ([#20](https://github.com/microsoft/tim-data-investigate-platform/issues/20)) ([1ac713f](https://github.com/microsoft/tim-data-investigate-platform/commit/1ac713ff11272406073245271d82f0d520c26b1a))

## [2.0.0](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v1.1.2...backend-v2.0.0) (2023-02-15)


### ⚠ BREAKING CHANGES

* modified the API for kusto queries.

### Features

* add get kusto schema. ([8ba47f7](https://github.com/microsoft/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))


### Bug Fixes

* validation of cluster uri with https. ([#17](https://github.com/microsoft/tim-data-investigate-platform/issues/17)) ([8ba47f7](https://github.com/microsoft/tim-data-investigate-platform/commit/8ba47f7880cb624457f6170e636958df0c4dc12e))

## [1.1.2](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v1.1.1...backend-v1.1.2) (2023-02-14)


### Bug Fixes

* enum conversion on query run status. ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* increase timeout for initializing couchbase db. ([#15](https://github.com/microsoft/tim-data-investigate-platform/issues/15)) ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* proper model validation instead of 500 exceptions. ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))
* return correct status for execute query and get results. ([adbc273](https://github.com/microsoft/tim-data-investigate-platform/commit/adbc273806b9ea115ccdf4e125cee79a4c271f74))

## [1.1.1](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v1.1.0...backend-v1.1.1) (2023-02-13)


### Bug Fixes

* fake fix to initiate rebuild. ([02637f0](https://github.com/microsoft/tim-data-investigate-platform/commit/02637f0cd3d4361d4a6b8e75f6b36870c89598b4))

## [1.1.0](https://github.com/microsoft/tim-data-investigate-platform/compare/backend-v1.0.5...backend-v1.1.0) (2023-02-10)


### Features

* reintroduce AAD authentication and OBO. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))


### Bug Fixes

* add time to live to couchbase upsert. ([887490c](https://github.com/microsoft/tim-data-investigate-platform/commit/887490cd973569df313ec5984696be1384f89016))
