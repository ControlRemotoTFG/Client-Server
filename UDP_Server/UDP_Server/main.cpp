#include <thread>
#include "Server.cpp"

void main()
{
	mtx.lock();
	std::cout << "Main empieza." << std::endl;
	mtx.unlock();

	thread server(server_thread);
	mtx.lock();
	std::cout << "Esperando server." << std::endl;
	mtx.unlock();

	server.join();
	
	mtx.lock();
	std::cout << "Server terminado." << std::endl;
	mtx.unlock();


}
