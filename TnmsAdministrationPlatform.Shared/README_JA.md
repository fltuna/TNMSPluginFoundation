# TnmsAdministrationPlatform

## なにこれ?

管理者情報を中央管理するための、共通化基盤です。

## 機能

### ユーザーデータの管理

ユーザーの管理者情報をDBを活用して、一元的に管理します。

### ノードベースの権限設定

このシステムでは、ノードベースの権限設定を採用しています。ノードは階層構造を持ち、各ノードに対して権限を設定できます。これにより、柔軟なアクセス制御が可能となります。

#### 仕組み

このシステムでは以下のような形でノードを定義し、活用します。

例：
- `tnms.admin`
- `tnms.administrationplatform.command.testcommand1`
- `tnms.administrationplatform.command.testcommand2`

そして、wildcardを利用して、以下のように権限を設定することも可能です。
- `*`: 全ての権限を持つ
- `tnms.*`: tnmsに関する全ての権限
- `tnms.administrationplatform.*`: AdministrationPlatformに関する全ての権限
- `tnms.administrationplatform.command.*`: AdministrationPlatformのコマンドに関する全ての権限


### 組み込み権限

- `tnms.admin`: AdministrationPlatformにおいて、ターゲティング可能かの有無を確認する際に使用されます。
  - 持たないユーザーは持っているユーザーをターゲティング出来ません。

## 使い方

### Config

TODO

## プラグイン開発

### 依存関係

NuGetから`TnmsAdministrationPlatform.Shared`をインストールします。

TODO

```xml
```

### 権限の検証

以下のようにして、AdminSystemを取得します。

```csharp


private IAdminManager _adminManager = null!;


public void OnAllModulesLoaded()
{
    var adminSystem = _sharedSystem.GetSharpModuleManager().GetRequiredSharpModuleInterface<IAdminManager>(IAdminManager.ModSharpModuleIdentity).Instance;
    _adminManager = adminSystem ?? throw new InvalidOperationException("TnmsAdministrationPlatform is not found! Make sure TnmsAdministrationPlatform is installed!");
}
```

以下のようにして権限を検証します。

なお、ワイルドカードを使用する際は、Root権限を除いて、`.*`で終わる必要があります。

```csharp
if (_adminManager.ClientHasPermission(player, "node.to.check"))

// Wildcardの例
if (_adminManager.ClientHasPermission(player, "node.to.*"))

// Root権限の例
if (_adminManager.ClientHasPermission(player, "*"))
```

詳細はコードドキュメントを確認してください。

