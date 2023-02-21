# TIM

TIM is a Kusto investigation platform that enables an analyst to quickly pivot between data sources; annotate their findings; and promotes collaboration through shared queries (pivots) and centralized tagged events.

## Getting Started

### Docker Compose

1. Download the docker compose YAML file.
```bash
curl -LO https://github.com/microsoft/tim-data-investigate-platform/raw/main/.docker/compose.yaml
```

2. Create or set the environment variables (refer to [Environment Variables](.docker/README.md#environment-variables)).

3. Run docker compose to download the latest images and deploy TIM locally.
```bash
docker compose up
```

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
