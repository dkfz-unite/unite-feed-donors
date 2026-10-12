# UNITE repository instructions

Before changing this repository, read the shared UNITE instructions and
relevant project documentation maintained in
[dkfz-unite/.github](https://github.com/dkfz-unite/.github/blob/main/README.md).

1. Prefer the local shared checkout: `../unite-meta/AGENTS.md`, relative to this
   repository root. A sibling `.github/AGENTS.md` or a user-provided path is also
   valid. Resolve its document links relative to that shared checkout.
2. Read the shared `AGENTS.md`, project overview, repository map, and topic
   documents relevant to the task, following the shared README's task index.
3. If no shared checkout is accessible, use the GitHub versions beginning with
   [shared AGENTS.md](https://github.com/dkfz-unite/.github/blob/main/AGENTS.md).
   If those are unavailable too, report the missing context and proceed only
   with what local documentation/code establishes; do not invent shared rules.
4. Read this repository's README and relevant local docs; inspect Git status.
   More specific instructions here supplement the shared guidance.

Keep common rules in `.github`; keep component details in this repository.

## Donors feed

Status: reviewed with the owner (2026-10-12).

The donors feed ingests donors with clinical data, project and study
assignments, and treatments; writes them to the domain database; and builds the
donors **and projects** indexes. It follows the same pattern as the
[omics feed](../unite-feed-omics/AGENTS.md) (submission staging, review, SQL
writers, indexing workers), without annotation or resources. Background:
[Data ingestion](../unite-meta/docs/data-ingestion.md),
[Processing pipeline](../unite-meta/docs/processing-pipeline.md),
[Domain model](../unite-meta/docs/domain.md#projects-and-visibility).

### Projects

| Project | Contents |
| --- | --- |
| `Unite.Donors.Feed.Web` | Controllers, submission models (donors, treatments), Mongo staging, workers and handlers (submissions, donors indexing, projects indexing) |
| `Unite.Donors.Feed` | SQL writers (`DonorsWriter`, `TreatmentsWriter`), `DonorsRemover`, `ProjectsRemover`, repositories |
| `Unite.Donors.Indices` | Donor and project index creators (`ProjectIndexCreator` also computes project statistics and members) |

There are no test projects.

### Build and run

```sh
dotnet build Unite.Donors.Feed.sln
dotnet run --project Unite.Donors.Feed.Web --launch-profile Unite.Donors.Feed.Web
```

- Local URL `http://localhost:5100`; Docker `donors.feed.unite.net`. Needs
  PostgreSQL, MongoDB and Elasticsearch.
- To test against the running platform, stop `unite.feed.donors` first; see
  [Running a service locally](../unite-meta/docs/development-validation.md#running-a-service-locally-instead-of-its-container).

### Configuration and API

Environment variables are in the [README](README.md); endpoints and models in
[Docs/api.md](Docs/api.md). Routes, task types and write semantics:

| Route | Task type | Notes |
| --- | --- | --- |
| `api/entries[/tsv]` | `DON` | Donor and clinical data, upsert. Provided `projects`, `studies` and `treatments` replace the donor's previous ones; a new donor without projects goes to `Other` |
| `api/treatments[/tsv]` | `DON_TRT` | Treatments, replaced per donor |
| `api/entry/{id}` (DELETE) | – | Deletes a donor with its data and index entry |
| `api/project/{id}` (DELETE) | – | Deletes a project and its index entry |
| `api/indexing/donors`, `api/indexing/projects` (POST) | – | Administrator reindexing |

`review=false` creates the submission task as `Prepared`. Replacing the
provided lists (projects, studies, treatments) while keeping omitted ones is
correct (owner, 2026-10-12). A general switch to "replace" semantics for all
data except SM is under consideration (Jira UND-1075).

### Projects and visibility

- Projects named in a submission are created if missing, **private**; only the
  default project `Other` is created public (`ProjectRepository.IsDefault`).
  See [Projects and visibility](../unite-meta/docs/domain.md#projects-and-visibility).
- The projects index carries visibility and members; Composer's project admin
  endpoints (members, `is-public`) change SQL and then call this feed's project
  reindexing. Keep that endpoint working when changing project indexing.
- Whether other feeds' submissions should wait for pending donor submissions,
  so that donors get their real projects before other data creates them in
  `Other`, is under consideration by the owner; see the omics feed notes.

### Shared libraries

`Unite.Data.Context` 12.10.20 (latest 12.10.33), `Unite.Indices.Context`
13.10.4 (latest 13.10.20), `Unite.Cache` 1.0.0 (latest 1.3.0),
`Unite.Essentials.Tsv` 4.1.0 (latest 4.2.0).

### CI

`docker-images.yml` builds amd64 and arm64 images under one manifest on pushes
to `main`.

### Known issues

Defects are tracked in the private Jira; see the shared
[Track defects](../unite-meta/AGENTS.md#track-defects).
