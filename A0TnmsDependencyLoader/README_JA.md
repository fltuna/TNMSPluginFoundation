# A0TnmsDependencyLoader

## なにこれ?

ModSharpには一括で依存関係のDLLをロードする機能がないため、それを可能にするモジュールです


## 使い方

### 1. A0TnmsDependencyLoaderをダウンロード

リリースから`A0TnmsDependencyLoader`をダウンロードして入れます。

### 2. 依存関係のDLLを各Moduleの`dependencies`フォルダに入れる

各Modulesの`dependencies`フォルダに各Moduleの依存関係のDLLを入れます。

### 3. サーバーを起動

起動時に`A0TnmsDependencyLoader`が各Moduleの`dependencies`フォルダを見に行き、DLLをロードします。
