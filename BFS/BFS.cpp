#include<iostream>
using namespace std;
 
#define N 4
 
//题目中所需要用到的节点
class Node {
public:
  Node(char *data, int g = 0, int h = 0): data(data), g(g), h(h) {}
  ~Node() { delete[] data; }
  int getF() const { return g + h; }
  int getG() const { return g; }
  int getH() const { return h; }
  char* getData() const { return data; }
  void setG(int g) { this->g = g; }
  void setH(int h) { this->h = h; }
  void setData(char *data) { this->data = data; }
  bool operator==(const Node& node) {
    for (int i = 0; i < N; ++i) {
      if (this->data[i] != node.getData()[i]) {
        return false;
      }
    }
    return true;
  }
  bool operator!=(const Node& node) {
    for (int i = 0; i < N; ++i) {
      if (this->data[i] != node.getData()[i]) {
        return true;
      }
    }
    return false;
  }
private:
  int g;
  int h;
  char* data;
};
 
//h(n)
int heuristic(Node* current, Node* goal) {
  int h = 0;
  for (int i = 0; i < N; ++i) {
    if (current->getData()[i] != goal->getData()[i]) {
      h++;
    }
  }
  return h;
}
 
//链表节点
struct ListNode {
  ListNode* next;
  Node* data;
};
 
void freeListNode(ListNode* node) {
  delete node->data;
  node->data = NULL;
  delete node;
  node = NULL;
}
 
//用链表实现优先队列
class List {
public:
  ~List() {
    ListNode* p = head;
    while (p != NULL) {
      p = head->next;
      freeListNode(head);
      head = p;
    }
  }
  ListNode* getHead() { return head; }
  void insert(ListNode* node) {
    if (head == NULL) {
      head = node;
      node->next = NULL;
    } else {
      if (node->data->getF() < head->data->getF()) {
        node->next = head;
        head = node;
      } else {
        ListNode* p = head;
        ListNode* q = p->next;
        while (q != NULL && node->data->getF() >= q->data->getF()) {
          p = q;
          q = q->next;
        }
        p->next = node;
        node->next = q;
      }
    }
  }
  void remove(ListNode* node) {
    if (head->data == node->data) {
      head = head->next;
    } else {
      ListNode* p = head;
      ListNode* q = head->next;
      while (q != NULL && q->data != node->data) {
        p = q;
        q = q->next;
      }
      if (q != NULL && q->next != NULL){
        p->next = q->next;
        q->next = NULL;
        freeListNode(q);
      } else if (q->next == NULL) {
        p->next;
        freeListNode(q);
      }
    }
  }
  ListNode* findNode(ListNode* node) {
    ListNode* p = head;
    while (p != NULL) {
      if (*node->data == *p->data) {
        return p;
      }
      p = p->next;
    }
    return NULL;
  }
  void pop() {
    ListNode* p = head;
    head = head->next;
  }
  bool empty() {
    if (head == NULL) {
      return true;
    }
    return false;
  }
private:
  ListNode* head = NULL;
};
 
void swapChar(char& a, char& b) {
  char temp = a;
  a = b;
  b = temp;
}
 
int main() {
 
  //初始化
  List open;
  List closed;
 
  Node* goal = new Node(new char[4]{'A', 'B', 'C', 'D'});
 
  ListNode* start_list_node = new ListNode();
  start_list_node->data = new Node(new char[4]{'C', 'B', 'A', 'D'}, 0);
  start_list_node->data->setH(heuristic(start_list_node->data, goal));
 
  open.insert(start_list_node);
 
  while (!open.empty()) {
    ListNode* top = open.getHead();
    open.pop();
 
    //输出遍历的节点
    cout << "[";
    for (int i = 0; i < N; ++i) {
      cout << top->data->getData()[i];
    }
    cout << "] " << top->data->getG() << "+" << top->data->getH() << endl;
 
    //找到目标结束
    if (*top->data == *goal) {
      break;
    }
 
    //生成子状态
    for (int i = N - 1; i > 0; --i) {
      char* temp = new char[N];
      for (int j = 0; j < N; ++j) {
        temp[j] = top->data->getData()[j];
      }
      swap(temp[i], temp[i - 1]);
      ListNode* child = new ListNode();
      child->data = new Node(temp, top->data->getG()+1);
      child->data->setH(heuristic(child->data, goal));
      if (open.findNode(child) == NULL && closed.findNode(child) == NULL) {
        open.insert(child);
      } else if (open.findNode(child) != NULL) {
        ListNode* old = open.findNode(child);
        if (child->data->getF() < old->data->getF()) {
          open.remove(old);
          open.insert(child);
        }
      } else if (closed.findNode(child) != NULL) {
        ListNode* old = closed.findNode(child);
        if (child->data->getF() < old->data->getF()) {
          closed.remove(old);
          open.insert(child);
        }
      }
    }
    closed.insert(top);
  }
 
  delete goal;
  system("PAUSE");
}