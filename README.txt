Proyecto realizado por Pablo Gómez Calvo y Sergio Juan Higuera Velasco.

Implementacion de un Cliente en Java que manda 2 ints serializados.
Estos ints se mandan a un Servidor C++ que los recibe y los muestra por pantalla.

Recordar mirar siempre la ip del pc antes de conectar el cliente, mirarla con ipconfig en el CMD.
Puerto por defecto 54000, no hacer caso a lo que pone por la consola, falta por arreglar.

Rama multithread: Servidor multihilo que ejecuta una aplicación con SDL y una servidor UDP de manera concurrente.
El dispositivo Android hace de cliente y la parte de abajo de la ventana de la aplicación es la zona "pulsable". Lo que significa que si pulsamos en una posición de la parte de abajo de la ventana de Android, guardaremos esa posición y cuando pulsemos el botón "CONNECT" se enviarán al servidor esas coordenadas guardadas con anterioridad.
En el servidor, se des-serializará esa posición y la aplicación SDL moverá en cuadrado rojo a la posición dada desde el Android.


Próximo paso: La ventana de Android se convertirá en un "mando" y en vez de enviarse los ints de donde se haya pulsado, se tratará en Android y se enviará directamente el mapeo de botones pulsados.
Con esto ya el cuadrado se moverá con autonomia propia y usaremos un primer mando con una prueba de SDL.
