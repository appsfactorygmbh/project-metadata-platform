# Changelog

## [2.1.0](https://github.com/appsfactorygmbh/project-metadata-platform/compare/v2.0.0...v2.1.0) (2026-08-05)


### Features

* **backend:** replace mediatR with own implementation of Mediator pattern ([b3aff3e](https://github.com/appsfactorygmbh/project-metadata-platform/commit/b3aff3e4e3f52b6d05d91213d5144f8cc8a919b2))

## [2.0.0](https://github.com/appsfactorygmbh/project-metadata-platform/compare/v1.0.0...v2.0.0) (2026-07-31)


### ⚠ BREAKING CHANGES

* **backend:** Now only SCIM Tokens can create new structure objects through user endpoints
* **backend:** Get Reqests now return allowed Action List
* **backend:** Adds Authorization Handling to all Request Handlers
* **backend:** Adds PipelineBehavior and Interceptor for Authorization Enforcement

### Features

* **backend:** Add Authorization Service ([70a55bb](https://github.com/appsfactorygmbh/project-metadata-platform/commit/70a55bbb899f03bbfe3523f02a9fd1ad974497dc))
* **backend:** Add new Scopes for ApiTokens ([79787c7](https://github.com/appsfactorygmbh/project-metadata-platform/commit/79787c7fb0e448fd030629b506244390dd3c0495))
* **backend:** Adds Authorization Handling to all Request Handlers ([d8ef9c8](https://github.com/appsfactorygmbh/project-metadata-platform/commit/d8ef9c8aa9276a3b2181b84e2defafa0f276b798))
* **backend:** Adds Handler for Authorization Exceptions ([ed7b58c](https://github.com/appsfactorygmbh/project-metadata-platform/commit/ed7b58ca0d5b1aa902967413279cc20c53a67c91))
* **backend:** Adds PipelineBehavior and Interceptor for Authorization Enforcement ([7084fdc](https://github.com/appsfactorygmbh/project-metadata-platform/commit/7084fdc4e174d59dc72516f95fa2a050af0d50da))
* **backend:** Get Reqests now return allowed Action List ([32f37f6](https://github.com/appsfactorygmbh/project-metadata-platform/commit/32f37f6c5929de8d630012f75e7d8499f2d757f4))
* **backend:** Now only SCIM Tokens can create new structure objects through user endpoints ([f79e793](https://github.com/appsfactorygmbh/project-metadata-platform/commit/f79e79388c4c453c6bcc5b981855dc5d30bb0eb4))
* **frontend:** add authorization ([5d0e860](https://github.com/appsfactorygmbh/project-metadata-platform/commit/5d0e860d874f87ac3c2e4ccba4484a337fe73954))
* **frontend:** Reworked Action Feedback; Added Forbidden handling ([81db53f](https://github.com/appsfactorygmbh/project-metadata-platform/commit/81db53f4a6f47dc36fc9e2eb7ec5939adb20988e))


### Bug Fixes

* **backend:** backend now refuses creating api tokens with a whitespace name ([dc014a9](https://github.com/appsfactorygmbh/project-metadata-platform/commit/dc014a99ebec238c873dd25b17aa01ef34353958))
* **backend:** fix user deletion for api tokens ([a8a2859](https://github.com/appsfactorygmbh/project-metadata-platform/commit/a8a2859319fc317cead24b87cea5ecf1d3b8de06))
* **backend:** mark user adresses as nullable ([e47b6ac](https://github.com/appsfactorygmbh/project-metadata-platform/commit/e47b6ac7f9e9672b1c22786afc7a8b6999aa9c4c))
* **backend:** now refuses creating users with whitespace only employee id ([445e3f9](https://github.com/appsfactorygmbh/project-metadata-platform/commit/445e3f9151cd488a034f975fee87ad9420115821))
* **frontend:** fix settings tab defaulting on mount ([15c9057](https://github.com/appsfactorygmbh/project-metadata-platform/commit/15c90573a6fb09d52dc05b839ac9a0f274fac452))
* **frontend:** fix splitpanes height in projects split view ([8efad65](https://github.com/appsfactorygmbh/project-metadata-platform/commit/8efad65b73d81ed2a78d5e50294115e6480a56d1))
* **frontend:** make user management default settings page; ([10b87b0](https://github.com/appsfactorygmbh/project-metadata-platform/commit/10b87b0d10525c8140ec3db19cf21ba83b774786))
* **frontend:** remove secret unfinished addPlugins button ([c07bbcd](https://github.com/appsfactorygmbh/project-metadata-platform/commit/c07bbcda23388911a4039cbb7a96e967b5cb3d52))

## 1.0.0 (2026-06-15)


### Features

* **backend:** update to dotnet 10 and update deps ([17f4cc6](https://github.com/appsfactorygmbh/project-metadata-platform/commit/17f4cc691abcb2d10d21ce66dad9a465163612bd))
* **frontend:** add version number in settings view ([e70be12](https://github.com/appsfactorygmbh/project-metadata-platform/commit/e70be12aff391dad4b728a8afabf34104bc41cfd))


### Bug Fixes

* **frontend:** EoC in create Project now resets when closing modal ([3402e60](https://github.com/appsfactorygmbh/project-metadata-platform/commit/3402e60ce87f28b2533422db94a1c2e71935abfc))
* **frontend:** fix project initialization ([bf41055](https://github.com/appsfactorygmbh/project-metadata-platform/commit/bf41055d53ceacb6353ac8270ccbcbc59b39bfa2))
* **frontend:** Further Fixes to make Azure Auth more stable ([6a87129](https://github.com/appsfactorygmbh/project-metadata-platform/commit/6a8712947ed079f7af74282dc4816d5cf1ed2685))
* **frontend:** update deps and update api models ([017de22](https://github.com/appsfactorygmbh/project-metadata-platform/commit/017de22b12560c42e5abadee4180493a72150b9f))
