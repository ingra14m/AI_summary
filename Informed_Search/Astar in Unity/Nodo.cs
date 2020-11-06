using UnityEngine;
using System.Collections;

public class Nodo {
	private int x;
	private int y;
	private Nodo padre;
	private int funcionG;
	private int heuristica;

	public Nodo(){
		
	}

	public Nodo(int _x, int _y){
		x = _x;
		y = _y;
	}

	public Nodo(int _x, int _y, Nodo _padre){
		x = _x;
		y = _y;
		padre = _padre;
	}

	public Nodo(int _x, int _y, int _funcionG, int _heuristica, Nodo _padre){
		x = _x;
		y = _y;
		funcionG = _funcionG;
		heuristica = _heuristica;
		padre = _padre;
	}

	public Vector2 GetCoordenadas(){
		return new Vector2(x,y);
	}

	public void SetCoordenadas(int _x, int _y){
		x = _x;
		y = _y;
	}

	public int GetFuncionG(){
		if(padre != null){
			return (funcionG + padre.funcionG);
		}else{
			return funcionG;	
		}
	}

	public void SetFuncionG(int nuevoValor){
		funcionG = nuevoValor;
	}

	public int GetHeuristica(){
		return heuristica;
	}

	public void SetHeuristica(int nuevoValor){
		heuristica = nuevoValor;
	}

	public int GetFuncionF(){
		return (funcionG + heuristica);
	}

	public Nodo GetPadre(){
		return padre;
	}

	public void SetPadre(Nodo nuevoPadre){
		padre = nuevoPadre;
	}
}
