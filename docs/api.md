# REST API

## Query

### Execute a query

```text
POST /query-execute
```

**Parameters:**

* `connection_id` string
* `data` dictionary

**Example request (Kusto):**

```json
{
    "connection_id": "c69e4539-5e13-4b73-84f3-9662fb4ee142",
    "data": {
        "cluster": "https://help.kusto.windows.net",
        "database": "Samples",
        "query": "StormEvents | take 1",
        "parameters": {
            "key": "value"
        }
    }
}
```

**Example request (Splunk):**

```json
{
    "connection_id": "c0aec099-ec07-45a4-86bd-b4e9fb26d708",
    "data": {
        "": "",
    }
}
```

**Response `202`:**

```json
{
    "query_id": "bee731fd-20be-4d06-bfa5-d8d41ab8647a"
}
```

### Retrieve query result

```text
GET /query-execute/:query_id
```

**Parameters:**

* `limit` int
* `page` int

**Response:**

```json
{
    "data": [
        {
            "key": "value"
        },
        {
            "key": "value"
        }
    ]
}
```

### List all labels for query result

```text
POST /queries/:query_id/labels
```

**Parameters:**

* `since` timestamp
* `limit` int
* `page` int

**Response:**

```json
[
    {
        "test": "TEST"
    }
]
```

### Auto-tag query result

Apply tags to a query result using the given auto-tag group.

```text
POST /queries/:query_id/autotag/:group_id
```

**Parameters:**

No parameters.

**Response:**

Alternate1:

```json
[
    [],
    ["tag1", "tag2"],
    []
]
```

Alternate2:

```json
{
    "1": ["tag1", "tag2"]
}
```

Alternate3:

```json
[
    {
        "row": 1,
        "tags": ["tag1", "tag2"]
    }
]
```

## Labels

### List all recent labels

```text
GET /labels
```

**Parameters:**

* `since` timestamp
* `limit` int
* `page` int

**Response:**

```json
[
    {
        "test": "TEST"
    }
]
```

### Retrieve label

```text
GET /labels/:event_id
```

**Parameters:**

No parameters.

**Response:**

```json
{
    "test": "TEST"
}
```

### Create label

```text
POST /labels
```

**Parameters:**

* `event_id` string
* `label` string

**Response:**

```json
{
    "test": "TEST"
}
```

### Delete label

```text
DELETE /labels/:event_id
```

**Parameters:**

No parameters.

**Response:**

```json
{
    "test": "TEST"
}
```
