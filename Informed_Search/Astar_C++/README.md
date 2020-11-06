# A-Star Algorithm
本示例时采用C++实现的高效的A-Star算法。相比于传统的A*算法，本示例的优化措施主要在于：快速判断路径节点是否在开启/关闭列表中、快速查找最小f值的节点以及优化路径节点频繁分配内存的问题。值得一提的是，本示例仅仅对对算法的程序实现做了尽力而为的优化，并没有对算法自身进行改良。

# 运行环境
支持c++11的编译器

# 使用示例
```c++
char maps[10][10] =
{
	{ 0, 1, 0, 0, 0, 1, 0, 0, 0, 0 },
	{ 0, 0, 0, 1, 0, 1, 0, 1, 0, 1 },
	{ 1, 1, 1, 1, 0, 1, 0, 1, 0, 1 },
	{ 0, 0, 0, 1, 0, 0, 0, 1, 0, 1 },
	{ 0, 1, 0, 1, 1, 1, 1, 1, 0, 1 },
	{ 0, 1, 0, 0, 0, 0, 0, 0, 0, 1 },
	{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
	{ 0, 0, 0, 0, 1, 0, 0, 0, 1, 0 },
	{ 1, 1, 0, 0, 1, 0, 1, 0, 0, 0 },
	{ 0, 0, 0, 0, 0, 0, 1, 0, 1, 0 },
};                                   // 构建搜索路径

// 搜索参数
AStar::Params param;
param.width = 10;
param.height = 10;
param.corner = false;
param.start = AStar::Vec2(0, 0);
param.end = AStar::Vec2(9, 9);
param.can_pass = [&](const AStar::Vec2 &pos)->bool
{
	return maps[pos.y][pos.x] == 0;
};

// 执行搜索
BlockAllocator allocator;
AStar algorithm(&allocator);
auto path = algorithm.find(param);       // 返回路径
```
