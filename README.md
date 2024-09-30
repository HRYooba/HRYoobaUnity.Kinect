# HRYoobaUnity.Kinect
## 1 インストール
### 1.1 パッケージマネージャーの設定
ProjectSetting/PackageManagerから以下のScopeRegistriesを設定
- Name: `package.openupm.com`
- URL: `https://package.openupm.com`
- Scope(s): 
  - `com.hryooba`
  - `com.cysha`
  - `org.nuget`

### 1.2 インストール
PackageManagerからMyRegistriesを選択しパッケージを入れる。

## 2 依存アセット
[Azure Kinect and Femto Bolt Examples for Unity](https://assetstore.unity.com/packages/tools/integration/azure-kinect-and-femto-bolt-examples-for-unity-149700?locale=ja-JP&srsltid=AfmBOoo_8lEgd75kuMkj7VQeyq96d2QtNxRAUVnECJ-fz2-SpbLB_-I6)

## 3 使い方
1. `Tools/HRYooba/Kinect/Create Json`から`areas_setting.json`と`kinects_setting.json`を作成  
![Create Json](https://github.com/user-attachments/assets/d1945546-2c94-4213-ad16-9489e71a4427)

2. Hierarchyから`HRYoobaKinect/RfilkovKinectManager`を作成  
![HRYoobaKinect/RfilkovKinectManager](https://github.com/user-attachments/assets/06e75c66-02b8-47ec-a381-bb2e88d7dbd2)

3. 起動  

<img src=https://github.com/user-attachments/assets/7bd35ae1-d9db-4cd4-bbb1-8fce6d7f97e6 width=50%>

4. PointCloudやJointを表示

<img src=https://github.com/user-attachments/assets/db031caf-aa74-42f8-a5d3-961b01c57267 width=50%>

## 4 設定ファイル
ビルドした場合はビルドしたファイルのexeと同じ階層にjsonファイルを配置してください。  
Editorの場合はプロジェクトの直下にjsonファイルを配置してください。

### 4.1 areas_setting.json
|パラメーター|型|デフォルト|詳細|
|:-|:-|:-|:-|
|settings|Setting[]||立ち位置エリアの設定|

Setting
|パラメーター|型|デフォルト|詳細|
|:-|:-|:-|:-|
|id|int|1|ID|
|position|Vector3|Vector3.zero|位置|
|radius|float|1.0|半径|

### 4.2 kinects_setting.json
|パラメーター|型|デフォルト|詳細|
|:-|:-|:-|:-|
|settings|Setting[]||Kinectの設定|

Setting
|パラメーター|型|デフォルト|詳細|
|:-|:-|:-|:-|
|id|string|1|デバイスID|
|position|Vector3|Vector3.zero|位置|
|euler_angles|Vector3|Vector3.zero|角度|
|min_depth_distance|float|0.5|depth最小距離 (0 ~ 10)|
|max_depth_distance|float|10.0|depth最大距離 (0 ~ 10)|
|body_tracking_sensor_orientation|BodyTrackingSensorOrientationType|"Default"|ボディトラッキング時のセンサー角度。Default = 横置き, Clockwise90 = 縦置き(時計回転), CounterClockwise90 = 縦置き(反時計回転), Flip180 = 横置き逆さ|
