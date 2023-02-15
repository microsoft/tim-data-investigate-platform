# Changelog

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
