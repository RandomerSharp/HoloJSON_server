# HoloJSON_server

JSON language support server for Taurus.

## 基本原理

本项目为可独立于[HoloCode](https://github.com/RandomerSharp/HoloCode)运行的、类[Language Server Protocol](https://microsoft.github.io/language-server-protocol/)的服务端程序，负责完成文本编辑器中与特定语言相关联的常用操作（如代码格式化、语法高亮等），采用[JSON-RPC 2.0](http://www.jsonrpc.org/specification)与客户端通讯。编辑器启动时，针对当前打开的文本文件的格式与内容在本地启动一个与之匹配的语言服务端，同一主机可以同时运行多个不同语言的服务端。

## 运行要求

- Node.js v8.8.1+，开发时对于v8.10.0 LTS以及v9.8.0版本均通过测试
- 理论上宿主操作系统支持Windows 10.0.16299+ 与Ubuntu 16.04.4 LTS
- 本服务端占用3000端口进行TCP通信

## 安装配置

1. 克隆项目到本地
2. 修改npm镜像源，或者以`npm --registry=https://registry.npm.taobao.org --disturl=http://npm.taobao.org/dist`运行（如果你对网速特别自信可以跳过此步骤）
3. `cd HoloJSON_server`
4. `npm install`
5. `npm run launch`
6. 启动浏览器，访问[本服务端RPC文档](localhost:3000/help)，若能正常访问即配置成功。

运行`launch`脚本后，将启用[nodemon](https://nodemon.io/)进行调试。（在调试时）不建议以其他方式启用服务端，`launch`脚本已经对调试启动进行了默认配置。

## 使用方法

~~抽根烟回来再写C#的调用例程。~~

[本服务端RPC文档](localhost:3000/help)上有浏览器内部POST交互的方法，并提供了[cURL](https://curl.haxx.se/)的命令。

`example`目录中的`Program.cs`是使用`C#`与本服务端交互的详细例程，附有说明性注释，并**给出了使用的JSON的格式**！

## 目前问题

目前要求待处理的本地文件必须事先保存在磁盘上，尚不能直接与`stdin`和`stdout`直接交互。 {2018-03-11}