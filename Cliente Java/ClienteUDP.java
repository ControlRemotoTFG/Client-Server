import java.net.*;
import java.io.*;
import java.util.*;

public class ClienteUDP {

  public static class PuntoXY{
      public PuntoXY(int x,int y){
        _x=x;
        _y=y;

      }

      public int getX(){
        return _x;

      }

      public int getY(){
        return _y;
      }

      private int _x;
      private int _y;
  }
  // Los argumentos proporcionan el mensaje y el nombre del servidor
  public static void main(String args[]) {
    Scanner S=new Scanner(System.in);
    try {
      DatagramSocket socketUDP = new DatagramSocket();
      //byte[] mensaje = args[0].getBytes();
      InetAddress hostServidor = InetAddress.getByName(args[0]);
      int puertoServidor = 6789;
      PuntoXY punto = new PuntoXY(10,20);
      int i=0;
      // Construimos un datagrama para enviar el mensaje al servidor
      while(i<10){
      int x = punto.getX();
      int y = punto.getY();
      byte[] mensaje = new byte[2];
      mensaje[0] = (byte) (x);
      mensaje[1] = (byte) (y);

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