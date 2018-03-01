# web 打印服务

### 功能列表

* 打印pdf文件
* 打印pdf Base64 字符串

### 接口

##### 简要描述

* 获取所有的打印机

##### 请求方式

* GET

##### 接口地址

* http://127.0.0.1:3119/Api/Index

##### 简要描述

* 打印文件 (目前只支持PDF打印）

##### 请求方式

* POST

##### 接口地址

* http://127.0.0.1:3119/Api/Print

#### 请求参数

|参数名|必填|类型|说明|
|:----|:----|:----|:----|
|mode|是|String|类型：base64 & file|
|type|是|String|文件类型：pdf |
|data|是|String|文件内容：如果mode是base64，那么转base64。如果mode是file转file路径|
|printName|是|String|打印机名称|
|rawKind|是|int|详细纸张类型：详细纸张类型与值的对照请看 [msdn](http://msdn.microsoft.com/zh-tw/library/system.drawing.printing.papersize.rawkind(v=vs.85).aspx） |
