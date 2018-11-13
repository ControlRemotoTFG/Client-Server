#pragma warning(disable:4996)
#include <iostream>
#include <WS2tcpip.h>


// Include the Winsock library (lib) file
#pragma comment (lib, "ws2_32.lib")

// Saves us from typing std::cout << etc. etc. etc.
using namespace std;

// Main entry point into the server

#include <mutex>          // std::mutex

static std::mutex mtx;

static void server_thread() {
	mtx.lock();
	std::cout << "Servidor empieza." << std::endl;
	mtx.unlock();


	////////////////////////////////////////////////////////////
// INITIALIZE WINSOCK
////////////////////////////////////////////////////////////

// Structure to store the WinSock version. This is filled in
// on the call to WSAStartup()
	WSADATA data;

	// To start WinSock, the required version must be passed to
	// WSAStartup(). This server is going to use WinSock version
	// 2 so I create a word that will store 2 and 2 in hex i.e.
	// 0x0202
	WORD version = MAKEWORD(2, 2);

	// Start WinSock
	int wsOk = WSAStartup(version, &data);
	if (wsOk != 0)
	{
		// Not ok! Get out quickly
		cout << "Can't start Winsock! " << wsOk;
		return;
	}

	////////////////////////////////////////////////////////////
	// SOCKET CREATION AND BINDING
	////////////////////////////////////////////////////////////

	// Create a socket, notice that it is a user datagram socket (UDP)
	SOCKET in = socket(AF_INET, SOCK_DGRAM, 0);

	int numPort = 54000;

	// Create a server hint structure for the server
	sockaddr_in serverHint;
	serverHint.sin_addr.S_un.S_addr = ADDR_ANY;
	serverHint.sin_family = AF_INET; // Address format is IPv4
	serverHint.sin_port = htons(numPort); // Convert from little to big endian

	// Try and bind the socket to the IP and port
	bind(in, (sockaddr*)&serverHint, sizeof(serverHint));
	
	
	mtx.lock();
	std::cout << "Connect to port number: " << numPort << '\n';
	mtx.unlock();

	////////////////////////////////////////////////////////////
	// MAIN LOOP SETUP AND ENTRY
	////////////////////////////////////////////////////////////

	sockaddr_in client; // Use to hold the client information (port / ip address)
	int clientLength = sizeof(client); // The size of the client information

	char buf[1024];

	// Enter a loop
	while (true)
	{
		mtx.lock();
		//std::cout << "Waiting..." << std::endl;
		mtx.unlock();
		ZeroMemory(&client, clientLength); // Clear the client structure
		ZeroMemory(buf, 1024); // Clear the receive buffer

		// Wait for message
		

		int bytesIn = recvfrom(in, buf, 1024, 0, (sockaddr*)&client, &clientLength);
		if (bytesIn == SOCKET_ERROR)
		{
			mtx.lock();
			cout << "Error receiving from client " << WSAGetLastError() << endl;
			mtx.unlock();
			continue;
		}

		// Display message and client info
		char clientIp[256]; // Create enough space to convert the address byte array
		ZeroMemory(clientIp, 256); // to string of characters

		// Convert from byte array to chars
		inet_ntop(AF_INET, &client.sin_addr, clientIp, 256);

		//Big-Endian decodification for 2 ints
		int x = 0;
		x = 0xFF & buf[3];
		x |= (0xFF & buf[2]) << 8;
		x |= (0xFF & buf[1]) << 16;
		x |= (0xFF & buf[0]) << 24;

		int y = 0;
		y = 0xFF & buf[7];
		y |= (0xFF & buf[6]) << 8;
		y |= (0xFF & buf[5]) << 16;
		y |= (0xFF & buf[4]) << 24;


		// Display the message / who sent it
		mtx.lock();
		cout << "Message recv from " << clientIp << " : X: " << x <<
			" Y: " << y << endl;
		mtx.unlock();
	}

	// Close socket
	closesocket(in);

	// Shutdown winsock
	WSACleanup();

	mtx.lock();
	std::cout << "Servidor finalizado" << std::endl;
	mtx.unlock();
}