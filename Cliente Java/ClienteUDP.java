import java.net.*;
import java.io.*;
import java.util.*;

public class ClienteUDP {

  // Los argumentos proporcionan el mensaje y el nombre del servidor
  public static void main(String args[]) {
    Scanner S=new Scanner(System.in);
    try {
      DatagramSocket socketUDP = new DatagramSocket();
      //byte[] mensaje = args[0].getBytes();
      InetAddress hostServidor = InetAddress.getByName(args[0]);
      int puertoServidor = 6789;
      String a;
      int i=0;
      // Construimos un datagrama para enviar el mensaje al servidor
      while(i<10){
      a=S.next();
      byte[] mensaje = a.getBytes();
      DatagramPacket peticion =
        new DatagramPacket(mensaje, mensaje.length, hostServidor,
                           puertoServidor);

      // Enviamos el datagrama
      socketUDP.send(peticion);
      i++;
      }
      // Cerramos el socket
      socketUDP.close();

    } catch (SocketException e) {
      System.out.println("Socket: " + e.getMessage());
    } catch (IOException e) {
      System.out.println("IO: " + e.getMessage());
    }
  }
}