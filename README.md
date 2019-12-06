## 基础数据结构
### Rule类
* features：int型HashSet，表示若干规则前件。
* isFinal：Boolean值，表示这条规则的后件是否为最终结论。
* conclusion：规则的后件，结论。
### Rules类
* allFeatures：String型List，其中记录所有的规则前件（包括中间结论，其index值就是features中对应的值）。
* finalConclusion：String型List，记录所有最终结论，index值作为conclusion的取值域。
* rules：Rule类HashSet，记录所有规则。

## 规则库
规则库使用Json文件保存，在使用时会被读入程序，反序列化成为一个Rules对象以供使用。程序中带有添加规则的功能，添加规则时只针对倒数第二个参数进行要求，添加规则时则会先向该对象中添加相应的记录，然后写回本地文件。

JSON文件示意：
```JSON
{
    "allFeatures":[String,],
    "finalConclusion":[String,],
    "rules":[
        {
            "features":[int,],
            "isFinal":boolean,
            "conclusion":int
        },
        {
            
        }
    ]
}
```
删除、修改规则的需求可以直接对JSON文件进行修改来满足。
## 综合数据库
综合数据库设计为一个int型的HashSet防止重复元素，从CLI中读到的参数在与allFeatures中的内容比对后转化为该特征所对应的index值存入综合数据库facts中，用于推理。
## 正向推理机
将综合数据库内容与反序列化的Rules对象中rules哈希集每个元素即每一条规则的features比对，当综合数据库中有该条规则所有的features即facts是features的超集时，判断该条规则对应的是否为最终结论，如果为最终结论则停止比较，因为在已经得到最终结论的情况下，继续推理得到另一个最终结论的可能性不太大，否则将中间结论加入到facts中，继续进行比较，直到facts不在是任何features的超集。
## 程序运行
* 注：程序以独立部署模式带win-x64运行时发布，体积可能比较大

指定规则库文件进行推理：
![指定规则库文件进行推理](https://i.loli.net/2019/12/06/INgA19mDHEibyjZ.jpg)
使用默认规则库文件进行推理：
![指定规则库文件进行推理](https://i.loli.net/2019/12/06/LVqYURXwxh8OEo5.jpg)
使用帮助：
![指定规则库文件进行推理](https://i.loli.net/2019/12/06/VgiAbOx69slWBGJ.jpg)
在程序中添加规则：
![在程序中添加规则](https://i.loli.net/2019/12/06/HGS4OW2sj3EoZtw.jpg)