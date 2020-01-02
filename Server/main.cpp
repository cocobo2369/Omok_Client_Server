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
		//this가 포인터인가봄!!
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
-서버소켓
-네트워크 설정 : WSAData, SOCKETARRDR_IN

-클라이언트 소켓 : 몇개가 들어올지 모르니 벡터로 선언
-클라이언트 cnt
*/

SOCKET serverSocket;

SOCKADDR_IN serverAddr;
WSAData wsaData;

vector<Client> clients;
int cntClient; //cnt가 client의 id로도 쓰임

vector<string> tokenize(string input, char delimiter) {
	vector<string> tokens;
	istringstream inputstream(input);
	string s;

	while (getline(inputstream, s, delimiter))
		tokens.push_back(s);

	return tokens;
}

enum RoomCnt { EMPTY = 0, ONE, FULL };

RoomCnt isRoomFull(int roomId) { //모든 vector를 다 돌아서 roomId를 찾는 방식이지만 hash 등으로 충분히 바꿀 수 있다.
	// room은 최대 2명만 들어갈 것이므로 0,1 은 false, 2는 true 로 하면 된다.
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

//게임 진행
void play(int roomId) {
	bool black = true;
	char* cmd = new char[Max];

	for (int i = 0; i < cntClient; i++) {
		if (clients[i].getRoomId() == roomId) {
			ZeroMemory(cmd, Max);
			if (black) {
				sprintf(cmd, "%s", "[Play]BLACK"); //sprintf 는 buffer에 출력내용을 저장할 수있는 기능인가보다. 즉 콘솔에 출력은 없다.
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
	/* 한 client로부터 좌표를 받을 건데, for문으로 roomId 에 해당하는 모든 client에게 (x,y)를 뿌리므로
	결국 한 쪽은 안받아도 되는데 받게 된다.
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

//thread 작업
void serverThread(Client* client) {
	/*
	client 추가되면 받는거랑
	client 한쪽요청되면 같은 room의 다른 client 도 '받을 수 있도록 하기'
	'받을 수 있도록' = 한 client로 부터 받고, 다른 client 한테 주고
	*/

	//주고 받고
	char* recvCmd = new char[Max];
	char* sendCmd = new char[Max];

	/*
	서버는 client로부터 receive를 받았는지가 우선체크해야할 상황이다.
	*/

	int recvSize = 0;
	while (true) {

		//1. client로부터 받은게 있다면, 받은 cmd가 무엇인지 해석해야한다.
		ZeroMemory(recvCmd, Max);
		if ((recvSize = recv(client->getSocket(), recvCmd, Max, 0) > 0)) {

			string recvString = string(recvCmd);
			vector<string> token = tokenize(recvString, ']');
			string data = token[1];
			//client로부터 받을 수 있는 명령어 [Enter] [Play] [Exit] [Axis] [Full] 이다.
			if (recvString.find("[Enter]") != -1) {

				//client가 요청한 방이 다 찼는가?
				string tmp;
				int IntClientRoomId = atoi(data.c_str());
				int RoomInCnt = 0;

				if ((RoomInCnt = isRoomFull(IntClientRoomId)) == FULL) //enum이 전역변수로 선언되어서 RoomCnt.FULL 하지 않아도 되나보다
					tmp = "[Full]";
				else {
					tmp = "[Enter]";
					/*해당 사용자의 방 접속 정보 갱신*/
					client->setRoomId(IntClientRoomId);
					clients[cntClient++] = *new Client(*client);
				}
				ZeroMemory(sendCmd, Max);
				sprintf(sendCmd, "%s", "[Full]");
				send(client->getSocket(), sendCmd, Max, 0);
				cout << "클라이언트 [" << client->getId() << "]:" << IntClientRoomId << " 번 방으로 접속함" << endl;

				if (RoomInCnt == ONE) //isRoomFull은 client가 추가되기전에 호출된 결과이므로 ONE일 때 이번에 client가 추가되고 게임이 바로 시작되는 것
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
			sprintf(sendCmd, "클라이언트 [%i]와의 연결이 끊어졌습니다.", client->getId());
			cout << sendCmd << endl;

			for (int i = 0; i < cntClient; i++) {
				if (clients[i].getId() == client->getId()) {
					//게임중이던 다른 client가 나간 경우
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

	cout << "[ C++ 오목 게임 서버 가동 ]" << endl;
	bind(serverSocket, (SOCKADDR*)&serverAddr, sizeof(serverAddr));
	listen(serverSocket, 32);

	int addrLen = sizeof(serverAddr);
	while (true) {
		SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, NULL);

		if (clientSocket = accept(serverSocket, (SOCKADDR*)&serverAddr, &addrLen)) {
			Client* client = new Client(cntClient, clientSocket);
			cout << "[ 새로운 사용자 접속 ]" << endl;
			CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)serverThread, (LPVOID)client, NULL, NULL); //client 마다 스레드가 생성되어 별도로 client가 처리된다.
			clients.push_back(*client);
			cntClient++;
		}
		Sleep(100);
	}
}