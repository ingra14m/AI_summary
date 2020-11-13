
# coding: utf-8

# In[5]:

import numpy as np
import operator
O=int(input(("请输入方阵的行/列数：")))
A=list(eval(x) for x in input("请输入起始状态").split())
B=list(eval(x) for x in input("请输入目标状态").split())
# A = list(map(int, A))
# B = list(map(int, B))
z=0

M=np.zeros((O,O))
N=np.zeros((O,O))
for i in range(O):                 # 把目标状态和起始状态分别输入N、M中
    for j in range(O):
        M[i][j]=A[z]
        N[i][j]=B[z]
        z = z+1
openlist=[]                       #open表,表示还没拓展的结点,放入的是一个state类

class State:
    def __init__(self,m):
        self.node=m#节点代表的状态
        self.f=0           #f(n)=g(n)+h(n)
        self.g=0           #g(n)
        self.h=0           #h(n)
        self.father=None#节点的父亲节点

init = State(M)#初始状态
goal=State(N)#目标状态

#启发函数
def h(s):                        # 当前状态到目标状态的代价，有位置不同则+1
    a=0
    for i in range(len(s.node)):
        for j in range(len(s.node[i])):
            if s.node[i][j]!=goal.node[i][j]:
                a=a+1
    return a

#对节点列表按照估价函数的值的规则排序
def list_sort(l):
    cmp=operator.attrgetter('f')
    l.sort(key=cmp)
    
#A*算法    
def A_star(s):
    global openlist                           #全局变量可以让open表进行时时更新
    openlist=[s]                              # 起始状态直接放进去
    while(openlist):                          #当open表不为空
        get=openlist[0]                       #取出open表的首节点  
        if (get.node==goal.node).all():       #判断是否与目标节点一致
            return get                        #在这里返回了目标节点
        openlist.remove(get)#将get移出open表
        
        #判断此时状态的空格位置，并且记录了a、b的值
        for a in range(len(get.node)):
            for b in range(len(get.node[a])):
                if get.node[a][b]==0:
                    break
            if get.node[a][b]==0:             # 跳出多重循环不用goto的无奈之举
                break
                
        #开始移动
        for i in range(len(get.node)):
            for j in range(len(get.node[i])):
                c = get.node.copy()
                if (i+j-a-b)**2 == 1:     # 找到直线距离为1的点
                    c[a][b]=c[i][j]
                    c[i][j]=0
                    new=State(c)
                    new.father=get        #此时取出的get节点成为新节点的父亲节点
                    new.g=get.g+1         #新节点与父亲节点的距离
                    new.h=h(new)          #新节点的启发函数值
                    new.f=new.g+new.h     #新节点的估价函数值
                    openlist.append(new)  #加入open表中
        list_sort(openlist)#排序 
# 递归打印路径
def printpath(f):
    if f is None:
        return
    #注意print()语句放在递归调用前和递归调用后的区别。放在后实现了倒叙输出
    printpath(f.father)
    print(f.node)

final=A_star(init)
if final:
    print("有解，解为：")
    printpath(final)        # 用递归来实现
else:
    print("无解")
           

