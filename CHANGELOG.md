# Changelog

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
