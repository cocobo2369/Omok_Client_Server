#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <windows.h>
#include <winsock.h>
#include < vector>
#include <sstream>
#pragma comment(lib, "ws2_32.lib")

#define Max 256

using namespace std;

class Client {
private:
	int id;
	int roomId;
	SOCKET socket;

public:
	Client(int id, SOCKET socket) {
		//this�� �������ΰ���!!
		this->id = id;
		this->socket = socket;
	}

	int getId() {
		return id;
	}

	int getRoomId() {
		return roomId;
	}

	SOCKET getSocket() {
		return socket;
	}

	void setRoomId(int roomId) {
		this->roomId = roomId;
	}
};

/*
-��������
-��Ʈ��ũ ���� : WSAData, SOCKETARRDR_IN

-Ŭ���̾�Ʈ ���� : ��� ������ �𸣴� ���ͷ� ����
-Ŭ���̾�Ʈ cnt
*/

SOCKET serverSocket;

SOCKADDR_IN serverAddr;
WSAData wsaData;

vector<Client> clients;
int cntClient; //cnt�� client�� id�ε� ����

vector<string> tokenize(string input, char delimiter) {
	vector<string> tokens;
	istringstream inputstream(input);
	string s;

	while (getline(inputstream, s, delimiter))
		tokens.push_back(s);

	return tokens;
}

enum RoomCnt { EMPTY = 0, ONE, FULL };

RoomCnt isRoomFull(int roomId) { //��� vector�� �� ���Ƽ� roomId�� ã�� ��������� hash ������ ����� �ٲ� �� �ִ�.
	// room�� �ִ� 2�� �� ���̹Ƿ� 0,1 �� false, 2�� true �� �ϸ� �ȴ�.
	int cnt = 0;
	for (int i = 0; i < cntClient; i++) {
		if (clients[i].getRoomId() == roomId) {
			cnt++;
			if (cnt == 2)
				return FULL;
		}
	}
	return cnt > 0 ? ONE : EMPTY;
}

void exit(int roomId) {
	char* cmd = new char[Max];

	for (int i = 0; i < cntClient; i++) {
		if (clients[i].getRoomId() == roomId) {
			ZeroMemory(cmd, Max);
			sprintf(cmd, "%s", "[Exit]");
			send(clients[i].getSocket(), cmd, Max, 0);
		}
	}
}

//���� ����
void play(int roomId) {
	bool black = true;
	char* cmd = new char[Max];

	for (int i = 0; i < cntClient; i++) {
		if (clients[i].getRoomId() == roomId) {
			ZeroMemory(cmd, Max);
			if (black) {
				sprintf(cmd, "%s", "[Play]BLACK"); //sprintf �� buffer�� ��³����� ������ ���ִ� ����ΰ�����. �� �ֿܼ� ����� ����.
				black = false;
			}
			else {
				sprintf(cmd, "%s", "[Play]WHITE");
			}
			send(clients[i].getSocket(), cmd, Max, 0);
		}
	}
}

void SendAxis(int roomId, int x, int y) {
	/* �� client�κ��� ��ǥ�� ���� �ǵ�, for������ roomId �� �ش��ϴ� ��� client���� (x,y)�� �Ѹ��Ƿ�
	�ᱹ �� ���� �ȹ޾Ƶ� �Ǵµ� �ް� �ȴ�.
	*/

	char* cmd = new char[Max];
	ZeroMemory(cmd, Max);
	for (int i = 0; i < cntClient; i++) {
		if (clients[i].getRoomId() == roomId) {
			string temp = "[Axis]" + to_string(x) + "," + to_string(y);
			sprintf(cmd, "%s", temp);
			send(clients[i].getSocket(), cmd, Max, 0);
		}
	}
}

//thread �۾�
void serverThread(Client* client) {
	/*
	client �߰��Ǹ� �޴°Ŷ�
	client ���ʿ�û�Ǹ� ���� room�� �ٸ� client �� '���� �� �ֵ��� �ϱ�'
	'���� �� �ֵ���' = �� client�� ���� �ް�, �ٸ� client ���� �ְ�
	*/

	//�ְ� �ް�
	char* recvCmd = new char[Max];
	char* sendCmd = new char[Max];

	/*
	������ client�κ��� receive�� �޾Ҵ����� �켱üũ�ؾ��� ��Ȳ�̴�.
	*/

	int recvSize = 0;
	while (true) {

		//1. client�κ��� ������ �ִٸ�, ���� cmd�� �������� �ؼ��ؾ��Ѵ�.
		ZeroMemory(recvCmd, Max);
		if ((recvSize = recv(client->getSocket(), recvCmd, Max, 0) > 0)) {

			string recvString = string(recvCmd);
			vector<string> token = tokenize(recvString, ']');
			string data = token[1];
			//client�κ��� ���� �� �ִ� ��ɾ� [Enter] [Play] [Exit] [Axis] [Full] �̴�.
			if (recvString.find("[Enter]") != -1) {

				//client�� ��û�� ���� �� á�°�?
				string tmp;
				int IntClientRoomId = atoi(data.c_str());
				int RoomInCnt = 0;

				if ((RoomInCnt = isRoomFull(IntClientRoomId)) == FULL) //enum�� ���������� ����Ǿ RoomCnt.FULL ���� �ʾƵ� �ǳ�����
					tmp = "[Full]";
				else {
					tmp = "[Enter]";
					/*�ش� ������� �� ���� ���� ����*/
					client->setRoomId(IntClientRoomId);
					clients[cntClient++] = *new Client(*client);
				}
				ZeroMemory(sendCmd, Max);
				sprintf(sendCmd, "%s", "[Full]");
				send(client->getSocket(), sendCmd, Max, 0);
				cout << "Ŭ���̾�Ʈ [" << client->getId() << "]:" << IntClientRoomId << " �� ������ ������" << endl;

				if (RoomInCnt == ONE) //isRoomFull�� client�� �߰��Ǳ����� ȣ��� ����̹Ƿ� ONE�� �� �̹��� client�� �߰��ǰ� ������ �ٷ� ���۵Ǵ� ��
					play(IntClientRoomId);
			}
			else if (recvString.find("[Axis]") != -1) {
				vector<string> data2 = tokenize(token[1], ',');
				int IntClientRoomId = atoi(data2[0].c_str());
				int x = atoi(data2[1].c_str());
				int y = atoi(data2[2].c_str());

				SendAxis(IntClientRoomId, x, y);
			}
			else if (recvString.find("[Play]") != -1) {
				int IntClientRoomId = atoi(data.c_str());
				play(IntClientRoomId);
			}
		}
		else {
			ZeroMemory(sendCmd, Max);
			sprintf(sendCmd, "Ŭ���̾�Ʈ [%i]���� ������ ���������ϴ�.", client->getId());
			cout << sendCmd << endl;

			for (int i = 0; i < cntClient; i++) {
				if (clients[i].getId() == client->getId()) {
					//�������̴� �ٸ� client�� ���� ���
					if (clients[i].getRoomId() != -1 && isRoomFull(client->getRoomId()) == FULL) {
						exit(clients[i].getRoomId());
					}
					clients.erase(clients.begin() + i);
					break;
				}
			}
			delete client;
			break;
		}
	}
}

int main() {
	WSAStartup(MAKEWORD(2, 2), &wsaData);
	serverSocket = socket(AF_INET, SOCK_STREAM, NULL);

	serverAddr.sin_addr.S_un.S_addr = inet_addr("127.0.0.1");
	serverAddr.sin_port = htons(9876);
	serverAddr.sin_family = AF_INET;

	cout << "[ C++ ���� ���� ���� ���� ]" << endl;
	bind(serverSocket, (SOCKADDR*)&serverAddr, sizeof(serverAddr));
	listen(serverSocket, 32);

	int addrLen = sizeof(serverAddr);
	while (true) {
		SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, NULL);

		if (clientSocket = accept(serverSocket, (SOCKADDR*)&serverAddr, &addrLen)) {
			Client* client = new Client(cntClient, clientSocket);
			cout << "[ ���ο� ����� ���� ]" << endl;
			CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)serverThread, (LPVOID)client, NULL, NULL); //client ���� �����尡 �����Ǿ� ������ client�� ó���ȴ�.
			clients.push_back(*client);
			cntClient++;
		}
		Sleep(100);
	}
}