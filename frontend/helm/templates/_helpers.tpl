{{/*
Expand the name of the chart.
*/}}
{{- define "frontend.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "frontend.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Select the version of the frontend to deploy
*/}}
{{- define "frontend.versionTag" -}}
{{- default .Chart.Version .Values.versionTag -}}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "frontend.chart" -}}
{{- printf "%s-%s" .Chart.Name (include "frontend.versionTag" .) | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "frontend.labels" -}}
helm.sh/chart: {{ include "frontend.chart" . }}
{{ include "frontend.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "frontend.selectorLabels" -}}
app.kubernetes.io/name: {{ include "frontend.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "frontend.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "frontend.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{- define "frontend.image.pullPolicy" -}}
{{- $pullPolicy := coalesce .Values.image.pullPolicy .Values.global.image.pullPolicy -}}
{{- if $pullPolicy }}
imagePullPolicy: {{ $pullPolicy | quote }}
{{- end -}}
{{- end -}}

{{- define "frontend.image.pullSecrets" -}}
{{- $pullSecrets := default (list) .Values.global.image.pullSecrets -}}
{{- if .Values.image.pullSecrets -}}
{{-   $pullSecrets = concat $pullSecrets .Values.image.pullSecrets -}}
{{- end -}}
{{- if $pullSecrets }}
imagePullSecrets:
{{-   range $index, $entry := $pullSecrets }}
- name: {{ $entry.name }}
{{-   end }}
{{- end }}
{{- end -}}
