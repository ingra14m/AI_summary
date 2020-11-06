using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingAStar : MonoBehaviour {
	//Dimensiones tablero
	public int ancho = 25;
	public int alto = 25;
	//Probabilidad de que una celda sea muro en vez de suelo (0-100)
	public int porcentajeMuro = 30;
	//Semilla aleatoria ? 
	public bool semillaRandom = false;
	//Modo de visualización paso a paso activado?
	public bool visualizacionPasoAPaso = false;
	//Se actualizará automaticamente el tablero despues X segundos o cuando se encuentre la solución?
	public bool actualizaAutomaticamente = false;
	//X segundos cada cuanto se actualiza el tablero 
	//(Funciona siempre y cuando no este activado el modo de visualización paso a paso)...
	[Range(1f,5f)]
	public float frecuenciaActualizacion = 1;
	//Almacena la semilla generadora del tablero
	public int semilla = 26;


	//Matriz que representa el mapa o tablero
	private int[,] tablero;
	//Se encontro una solución al problema? 
	//(Variable simbólica que permite actualizar el tablero 
	//cuando se elige visualizar la solución paso a paso)
	private bool solucionEncontrada;
	//Posición de la celda de origen
	private Vector2 celdaOrigen;
	//Posición de la celda de destino
	private Vector2 celdaDestino;
	//Lista de celdas que son parte del suelo
	private List<Vector2> celdasLibres = new List<Vector2>();
	//Lista que almacena posibles celdas que pueden ser solución del camino final
	private List<Nodo> listaParcial = new List<Nodo>();
	//Lista que almacena las celdas que forman parte de la solución
	private List<Nodo> listaFinal = new List<Nodo>();

	void Awake(){
		solucionEncontrada = false;
		if(actualizaAutomaticamente){
			InvokeRepeating("ActualizarMapa", 0.1f, frecuenciaActualizacion);
		}else{
			ActualizarMapa();
		}	
	}

	public void ActualizarMapa(){
		if(!solucionEncontrada){
			GenerarMapa();

			if(visualizacionPasoAPaso)
				StartCoroutine(ComenzarBusquedaCaminoPasoAPaso());
			else
				ComenzarBusquedaCamino();
		}
	}

	void GenerarMapa(){
		solucionEncontrada = true;
		//Inicialización variables
		tablero = new int[ancho,alto];
		celdasLibres.Clear();

		if(tablero != null){

			//Asignación semilla...
			if(semillaRandom){
				semilla = Random.Range(int.MinValue, int.MaxValue);
			}else{
				Random.seed = semilla;
			}

			//Rellenar el tablero con suelo y muro tomando en cuenta la probabilidad de que salgan muros.
			for (int x = 0; x < ancho; x++) {
				for (int y = 0; y < alto; y++) {
					float probabilidad = Random.Range(0f,100f);
					if(probabilidad <= porcentajeMuro){
						tablero[x,y] = 1;
					}else{
						tablero[x,y] = 0;
						celdasLibres.Add(new Vector2(x,y));
					}
				}
			}
		}
		//Se procede a posicionar el punto de origen y destino en el tablero.
		PosicionarInicioFin();
	}

	void PosicionarInicioFin(){
		//Punto inicial
		int indiceInicio = Random.Range(0,celdasLibres.Count);
		tablero[(int)celdasLibres[indiceInicio].x, (int)celdasLibres[indiceInicio].y] = 2;
		celdaOrigen = new Vector2(celdasLibres[indiceInicio].x, celdasLibres[indiceInicio].y);
		celdasLibres.RemoveAt(indiceInicio);

		//Punto final
		int indiceFinal = Random.Range(0,celdasLibres.Count);
		tablero[(int)celdasLibres[indiceFinal].x, (int)celdasLibres[indiceFinal].y] = 3;
		celdaDestino = new Vector2(celdasLibres[indiceFinal].x, celdasLibres[indiceFinal].y);
		celdasLibres.RemoveAt(indiceFinal);
	}

	IEnumerator ComenzarBusquedaCaminoPasoAPaso(){
		//Debug.Log("Comenzando busqueda");
		//Se inicialización las listas.
		listaParcial.Clear();
		listaFinal.Clear();

		//Se asigna el nodo inicial, es decir el origen del camino.
		Nodo inicio = new Nodo((int)celdaOrigen.x, (int)celdaOrigen.y);
		listaParcial.Add(inicio);

		//Debug.Log("Coordenada de Inicio: " + coordenadaInicio);
		//Debug.Log("Coordenada de Fin: " + coordenadaFin);

		//Se revisan los 8 vecinos alrededor del nodo inicial
		for (int x = (int)celdaOrigen.x - 1; x <= (int)celdaOrigen.x + 1; x++) {
			for (int y = (int)celdaOrigen.y - 1; y <= (int)celdaOrigen.y + 1; y++) {
				//Debug.Log("Revisando coordenadas: " +x + " , " +y);

				//Siempre y cuando no este fuera del tablero...
				if(x >= 0 && x < ancho && y >= 0 && y < alto){

					//Si la celda es suelo, destino o posible camino.
					if(tablero[x,y] == 0 || tablero[x,y] == 3 || tablero[x,y] == 5){
						Nodo vecino = new Nodo(x,y,inicio);
						int funcionG = CalcularFuncionG(vecino);
						int heuristica = CalcularHeuristica(vecino);
						vecino.SetFuncionG(funcionG);
						vecino.SetHeuristica(heuristica);
						listaParcial.Add(vecino);

						//Asignación para visualización de los posibles caminos.
						if(tablero[x,y] != 3){
							tablero[x,y] = 5;	
						}

						yield return new WaitForSeconds(0.01f);
					}
				}
			}
		}

		listaParcial.Remove(inicio);
		//Debug.Log("Nodo inicio tiene: " +listaParcial.Count + " vecinos disponibles.");
		listaFinal.Add(inicio);

		//Se sigue buscando la solución mientras la lista parcial no este vacía o 
		//no se haya encontrado la solución final.
		while(listaParcial.Count > 0 && !SeEncontroSolucion()){
			int indiceNodoFuncionFMasBaja = ObtenerIndiceNodoConFuncionFMasBaja();
			Nodo nodo = listaParcial[indiceNodoFuncionFMasBaja];
			//Debug.Log("----------------------------");
			//Debug.Log("Posicion Nodo: " + nodo.GetCoordenadas());
			//Debug.Log("Funcion F Nodo: " +nodo.GetFuncionF());
			listaParcial.Remove(nodo);
			listaFinal.Add(nodo);

			//Se revisan los 8 vecinos del nodo actual.
			for (int x = (int)nodo.GetCoordenadas().x - 1; x <= (int)nodo.GetCoordenadas().x + 1; x++) {
				for (int y = (int)nodo.GetCoordenadas().y - 1; y <= (int)nodo.GetCoordenadas().y + 1; y++) {
					//Debug.Log("Revisando coordenadas: " +x + " , " +y);

					//Si la celda visitada ya esta en la solución final se salta.
					if(NodoEstaEnListaFinal(x,y)){
						//Debug.Log("Ya estaba en la lista final");
						continue;
					}

					//Mientras no este fuera del tablero...
					if(x >= 0 && x < ancho && y >= 0 && y < alto){

						//Si la celda es suelo, destino o posible camino.
						if(tablero[x,y] == 0 || tablero[x,y] == 3 || tablero[x,y] == 5){

							//Si la celda actual no esta en los posibles caminos..
							if(!NodoEstaEnListaParcial(x,y)){
								//Debug.Log("NO esta en la lista parcial, se agrega como vecino.");

								Nodo vecino = new Nodo(x,y,nodo);
								int funcionG = CalcularFuncionG(vecino);
								int heuristica = CalcularHeuristica(vecino);
								vecino.SetFuncionG(funcionG);
								vecino.SetHeuristica(heuristica);

								//Asignación para visualización de los posibles caminos.
								if(tablero[x,y] != 3){
									tablero[x,y] = 5;	
								}

								listaParcial.Add(vecino);	
							}else{
								//Debug.Log("SI esta en la lista parcial.");

								Nodo vecino = NodoEnListaParcial(x,y);
								Nodo auxiliar = new Nodo(x,y,nodo);
								int funcionG = CalcularFuncionG(auxiliar);
								auxiliar.SetFuncionG(funcionG);

								//Si estaba en la solución parcial entonces se compara 
								//con la función G del camino actual, para ver cual es mejor.
								if(auxiliar.GetFuncionG() < vecino.GetFuncionG()){
									//Debug.Log("El camino actual es mejor que el suyo.");
									vecino.SetPadre(nodo);
									int nuevaFuncionG = CalcularFuncionG(vecino);
									int nuevaHeuristica = CalcularHeuristica(vecino);
									vecino.SetFuncionG(nuevaFuncionG);
									vecino.SetHeuristica(nuevaHeuristica);
								}
							}
							yield return new WaitForSeconds(0.01f);
						}
					}
				}
			}
		}

		//Debug.Log("------------------------");

		if(SeEncontroSolucion()){
			//Debug.Log("Solucion encontrada");
			PintarCaminoSolucion();
		}else{
			//Debug.Log("No hay solucion encontrada");
		}
		yield return new WaitForSeconds(1f);
		//Recién en este punto si esta activado la actualización automática, se permite...
		solucionEncontrada = false;
		yield return null;
	}

	void ComenzarBusquedaCamino(){
		//Debug.Log("Comenzando busqueda");
		//Se inicialización las listas.
		listaParcial.Clear();
		listaFinal.Clear();

		//Se asigna el nodo inicial, es decir el origen del camino.
		Nodo inicio = new Nodo((int)celdaOrigen.x, (int)celdaOrigen.y);
		listaParcial.Add(inicio);

		//Debug.Log("Coordenada de Inicio: " + coordenadaInicio);
		//Debug.Log("Coordenada de Fin: " + coordenadaFin);

		//Se revisan los 8 vecinos alrededor del nodo inicial
		for (int x = (int)celdaOrigen.x - 1; x <= (int)celdaOrigen.x + 1; x++) {
			for (int y = (int)celdaOrigen.y - 1; y <= (int)celdaOrigen.y + 1; y++) {
				//Debug.Log("Revisando coordenadas: " +x + " , " +y);

				//Siempre y cuando no este fuera del tablero...
				if(x >= 0 && x < ancho && y >= 0 && y < alto){

					//Si la celda es suelo, destino o posible camino.
					if(tablero[x,y] == 0 || tablero[x,y] == 3){
						Nodo vecino = new Nodo(x,y,inicio);
						int funcionG = CalcularFuncionG(vecino);
						int heuristica = CalcularHeuristica(vecino);
						vecino.SetFuncionG(funcionG);
						vecino.SetHeuristica(heuristica);
						listaParcial.Add(vecino);
					}
				}
			}
		}

		listaParcial.Remove(inicio);
		//Debug.Log("Nodo inicio tiene: " +listaParcial.Count + " vecinos disponibles.");
		listaFinal.Add(inicio);

		//Se sigue buscando la solución mientras la lista parcial no este vacía o 
		//no se haya encontrado la solución final.
		while(listaParcial.Count > 0 && !SeEncontroSolucion()){
			int indiceNodoFuncionFMasBaja = ObtenerIndiceNodoConFuncionFMasBaja();
			Nodo nodo = listaParcial[indiceNodoFuncionFMasBaja];
			//Debug.Log("----------------------------");
			//Debug.Log("Posicion Nodo: " + nodo.GetCoordenadas());
			//Debug.Log("Funcion F Nodo: " +nodo.GetFuncionF());
			listaParcial.Remove(nodo);
			listaFinal.Add(nodo);

			//Se revisan los 8 vecinos del nodo actual.
			for (int x = (int)nodo.GetCoordenadas().x - 1; x <= (int)nodo.GetCoordenadas().x + 1; x++) {
				for (int y = (int)nodo.GetCoordenadas().y - 1; y <= (int)nodo.GetCoordenadas().y + 1; y++) {
					//Debug.Log("Revisando coordenadas: " +x + " , " +y);

					//Si la celda visitada ya esta en la solución final se salta.
					if(NodoEstaEnListaFinal(x,y)){
						//Debug.Log("Ya estaba en la lista final");
						continue;
					}

					//Mientras no este fuera del tablero...
					if(x >= 0 && x < ancho && y >= 0 && y < alto){

						//Si la celda es suelo, destino o posible camino.
						if(tablero[x,y] == 0 || tablero[x,y] == 3){

							//Si la celda actual no esta en los posibles caminos..
							if(!NodoEstaEnListaParcial(x,y)){
								//Debug.Log("NO esta en la lista parcial, se agrega como vecino.");

								Nodo vecino = new Nodo(x,y,nodo);
								int funcionG = CalcularFuncionG(vecino);
								int heuristica = CalcularHeuristica(vecino);
								vecino.SetFuncionG(funcionG);
								vecino.SetHeuristica(heuristica);
								listaParcial.Add(vecino);	
							}else{
								//Debug.Log("SI esta en la lista parcial.");

								Nodo vecino = NodoEnListaParcial(x,y);
								Nodo auxiliar = new Nodo(x,y,nodo);
								int funcionG = CalcularFuncionG(auxiliar);
								auxiliar.SetFuncionG(funcionG);

								//Si estaba en la solución parcial entonces se compara 
								//con la función G del camino actual, para ver cual es mejor.
								if(auxiliar.GetFuncionG() < vecino.GetFuncionG()){
									//Debug.Log("El camino actual es mejor que el suyo.");
									vecino.SetPadre(nodo);
									int nuevaFuncionG = CalcularFuncionG(vecino);
									int nuevaHeuristica = CalcularHeuristica(vecino);
									vecino.SetFuncionG(nuevaFuncionG);
									vecino.SetHeuristica(nuevaHeuristica);
								}
							}
						}
					}
				}
			}
		}

		//Debug.Log("------------------------");

		if(SeEncontroSolucion()){
			//Debug.Log("Solucion encontrada");
			PintarCaminoSolucion();
		}else{
			//Debug.Log("No hay solucion encontrada");
		}
		//Recién en este punto si esta activado la actualización automática, se permite...
		solucionEncontrada = false;
	}

	int CalcularFuncionG(Nodo nodo){
		if(nodo.GetPadre().GetCoordenadas().x == nodo.GetCoordenadas().x || nodo.GetPadre().GetCoordenadas().y == nodo.GetCoordenadas().y){
			return 10;
		}else{
			return 14;
		}
	}

	int CalcularHeuristica(Nodo nodo){
		return (10 * ((int)(Mathf.Abs((int)nodo.GetCoordenadas().x - (int)celdaDestino.x)) + (int)(Mathf.Abs((int)nodo.GetCoordenadas().y - (int)celdaDestino.y))));
	}

	int ObtenerIndiceNodoConFuncionFMasBaja(){
		int minimoPuntaje = int.MaxValue;
		int indiceNodo = -1;

		for (int indice = 0; indice < listaParcial.Count; indice++) {
			if(listaParcial[indice].GetFuncionF() < minimoPuntaje){
				indiceNodo = indice;
				minimoPuntaje = listaParcial[indice].GetFuncionF();
			}
		}

		return indiceNodo;
	}

	bool NodoEstaEnListaFinal(int x, int y){
		for (int indice = 0; indice < listaFinal.Count; indice++) {
			if(x == listaFinal[indice].GetCoordenadas().x && y == listaFinal[indice].GetCoordenadas().y){
				return true;
			}
		}

		return false;
	}

	bool NodoEstaEnListaParcial(int x, int y){
		for (int indice = 0; indice < listaParcial.Count; indice++) {
			if(x == listaParcial[indice].GetCoordenadas().x && y == listaParcial[indice].GetCoordenadas().y){
				return true;
			}
		}

		return false;
	}

	Nodo NodoEnListaParcial(int x, int y){
		for (int indice = 0; indice < listaParcial.Count; indice++) {
			if(x == listaParcial[indice].GetCoordenadas().x && y == listaParcial[indice].GetCoordenadas().y){
				return listaParcial[indice];
			}
		}

		return null;
	}

	bool SeEncontroSolucion(){
		for (int indice = 0; indice < listaParcial.Count; indice++) {
			if(listaParcial[indice].GetCoordenadas().x == (int)celdaDestino.x && listaParcial[indice].GetCoordenadas().y == (int)celdaDestino.y){
				return true;
			}
		}

		return false;
	}

	void PintarCaminoSolucion(){
		Nodo solucion = new Nodo();
		for (int indice = 0; indice < listaParcial.Count; indice++) {
			if(listaParcial[indice].GetCoordenadas().x == (int)celdaDestino.x && listaParcial[indice].GetCoordenadas().y == (int)celdaDestino.y){
				solucion = listaParcial[indice];
				break;
			}
		}

		Nodo auxiliar = solucion;
		while(auxiliar.GetPadre() != null){
			if(tablero[(int)auxiliar.GetCoordenadas().x, (int)auxiliar.GetCoordenadas().y] != 2 && tablero[(int)auxiliar.GetCoordenadas().x, (int)auxiliar.GetCoordenadas().y] != 3){
				tablero[(int)auxiliar.GetCoordenadas().x, (int)auxiliar.GetCoordenadas().y] = 4;	
			}
			auxiliar = auxiliar.GetPadre();
		}
	}

	void OnDrawGizmos(){
		if(tablero != null){
			for (int x = 0; x < ancho; x++) {
				for (int y = 0; y < alto; y++) {
					//Representa el suelo
					if(tablero[x,y] == 0){
						Gizmos.color = Color.white;
					}
					//Representa las murallas
					if(tablero[x,y] == 1){
						Gizmos.color = Color.black;
					}
					//Representa la celda de origen
					if(tablero[x,y] == 2){
						Gizmos.color = Color.blue;
					}
					//Representa la celda de destino
					if(tablero[x,y] == 3){
						Gizmos.color = Color.red;
					}
					//Representa el camino optimo encontrado
					if(tablero[x,y] == 4){
						Gizmos.color = Color.magenta;
					}
					//Representa los posibles caminos
					if(tablero[x,y] == 5){
						Gizmos.color = Color.green;
					}
					Gizmos.DrawCube(new Vector3(x,y,0f), new Vector3(0.9f, 0.9f, 0.9f));
				}
			}
		}
	}
}
