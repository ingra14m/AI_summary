# 机器树 Uniform Cost Search
from queue import PriorityQueue  # 优先级队列


def ucs(graph, home, guest):
    if home not in graph:  # 起点不在Frontier集中
        raise TypeError(str(home) + ' not found in graph !')
        return
    if guest not in graph:  # 终点不在Frontier集中
        raise TypeError(str(guest) + ' not found in graph !')
        return
    # visited = []
    queue = PriorityQueue()
    queue.put((0, [home]))  # 0表示优先级，home是放入的元素
    # visited.append(home)

    while not queue.empty():
        # print ("Currnet queue is:",queue.queue)

        node = queue.get()  # 会取出queue里面cost最小的那个
        # print ("Node:",node)

        # 避免重复搜索
        visited = node[1]
        current = node[1][len(node[1]) - 1]
        # current = node[1][0]
        # print ("Current:",current)
        if guest in node[1]:
            print("Path found: " + str(node[1]) + ", Cost = " + str(node[0]))
            break

        cost = node[0]
        for neighbor in graph[current]:
            if neighbor in visited:
                continue
            temp = node[1][:]
            # print ("Temp:",temp)
            temp.append(neighbor)
            # print ("Temp append neighbor:",temp)
            queue.put((cost + graph[current][neighbor], temp))
    # print (queue)


def main():
    file = open("maps.txt", "r")               # 读取数据
    lines = file.readlines()
    # 构建一个词典，来保存整个图
    graph = {}  # 字典的元素还是字典
    for line in lines:  # 输入原始的数据
        # print (line)
        token = line.split()
        node = token[0]  # 城市名
        graph[node] = {}

        for i in range(1, len(token) - 1, 2):
            graph[node][token[i]] = int(token[i + 1])  # 城市以及对应的距离
    # graph = retrieval()
    # print (len(graph["Anyang"]))
    ucs(graph, "Anyang", "HongKong")  # 表示起点和终点，以及使用的中间存储的字典


if __name__ == "__main__":
    main()