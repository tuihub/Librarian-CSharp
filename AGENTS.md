# Librarian-CSharp Agents 概览

本项目以 gRPC 为核心，围绕多套“角色”式服务协同工作。下表梳理主要组件、入口位置与职责，便于快速定位代码：

| 组件 | 项目/入口 | 职责与要点 |
| --- | --- | --- |
| **Sephirah Server** | `Librarian.Sephirah.Server` → `StartUp`/`Program` | 核心服务宿主，开放 gRPC（含 JSON Transcoding）并在开发环境启用 Swagger/Reflection。挂载 `SephirahService` 与 `AngelaService`；内置 EF Core 数据库迁移、JWT 身份验证（多受众）、速率限制；可选 Consul（Porter 发现）与 MassTransit（InMemory/RabbitMQ）。 |
| **Sephirah Service 库** | `Librarian.Sephirah` | 业务主线：`Tiphereth` 处理令牌、设备与账号绑定；`Binah` 处理 MinIO 文件上传/下载；`PullMetadataService` 定时拉取第三方元数据；`SephirahService` 统筹并通过 `SephirahContext` 记录 Porter 列表。 |
| **Angela Service 库** | `Librarian.Angela` | 管理向 API（与 Sephirah Server 同进程部署）。`Tiphereth` 负责用户创建、列表、令牌刷新与本地管理员检查；`Sentinel` 维护 Sentinel 节点；`StoreApp`/`Gebura` 处理商店应用与元数据查询。 |
| **Porter Server** | `Librarian.Porter.Server` → `Program` | 元数据采集/账号拉取插件宿主。暴露 gRPC + Reflection，按 `PorterConfig` 启用 Steam/Bangumi/VNDB 等；`ServicesUtil` 注入第三方客户端，`ConsulUtil` 在启用时注册健康检查（默认 gRPC 5000、Health 5001）。 |
| **Porter Service 库** | `Librarian.Porter` | 具体处理逻辑：`Gebura` 负责应用信息抓取/解析，`Tiphereth` 处理账号获取，`Common` 提供启用/信息查询等端点，服务由 Porter Server 映射。 |
| **Sentinel Agent 库** | `Librarian.Sentinel` | 面向边缘探针（Sentinel）的 gRPC 处理：`Gebura` 汇报探针信息与应用二进制，`Tiphereth` 心跳与令牌刷新。由上层宿主进程调用。 |
| **Angela Blazor Server** | `Librarian.Angela.BlazorServer` | 管理后台前端，默认通过 `appsettings.json` 的 `SephirahApi`/`AngelaApi` 指向同一后端（本地 https://localhost:5148）。 |
| **共享与基础设施** | `Librarian.Common`、`Librarian.ThirdParty`、`Librarian.Common.Migrations.*` | 公共模型/配置/工具（如 `SystemConfig` 的 MinIO、Static Porter、AngelaTrustedIPs 等），第三方 API 客户端，以及多数据库迁移程序集（SQLite/MySQL/PostgreSQL）。 |

## 运行与配置速览
- **Sephirah 配置**：`Librarian.Sephirah.Server/appsettings.json` 定义数据库、JWT、多租户令牌过期、MinIO、静态 Porter 列表、Consul 与 MassTransit 参数。
- **Porter 配置**：`Librarian.Porter.Server/appsettings.json` 配置启用的平台、速率限制、Consul 注册信息及端口。
- **认证/授权**：Sephirah Server 使用多受众 JWT（Access/Refresh/Upload/Download），`AngelaAuthorizationHandler` 提供管理面访问控制。
- **消息与发现**：`SephirahContext` 维护通过 Consul 与静态配置发现的 Porter 节点，MassTransit 可在 InMemory 或 RabbitMQ 之间切换。

## 开发者提示
- gRPC 端点统一由各自的 Server 项目映射；Angela/Sephirah 服务同进程托管在 `Librarian.Sephirah.Server`。
- 静态 Porter（`SystemConfig.StaticPorterInstances`）可用于无 Consul 环境；内置标签（如 `platform:*`）在 `Sephirah` 侧用于路由。
- Sephirah 启动时会执行数据库迁移；Binah 文件流依赖 MinIO 配置；开发环境可启用 gRPC-Web 与 Swagger 便于调试。
