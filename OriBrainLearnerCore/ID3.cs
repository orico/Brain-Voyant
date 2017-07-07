using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text; 

namespace OriBrainLearnerCore
{
	/// <summary>
	/// Classe que representa um atributo utilizado na classe de decisדo
	/// </summary>
	public class Attribute
	{
		ArrayList mValues;
		string mName;
		object mLabel;

		/// <summary>
		/// Inicializa uma nova instגncia de uma classe Atribute
		/// </summary>
		/// <param name="name">Indica o nome do atributo</param>
		/// <param name="values">Indica os valores possםveis para o atributo</param>
		public Attribute(string name, string[] values)
		{
			mName = name;
			mValues = new ArrayList(values);
			mValues.Sort();
		}

		public Attribute(object Label)
		{
			mLabel = Label;
			mName = string.Empty;
			mValues = null;
		}

		/// <summary>
		/// Indica o nome do atributo
		/// </summary>
		public string AttributeName
		{
			get
			{
				return mName;
			}
		}

		/// <summary>
		/// Retorna um array com os valores do atributo
		/// </summary>
		public string[] values
		{
			get
			{
				if (mValues != null)
					return (string[])mValues.ToArray(typeof(string));
				else
					return null;
			}
		}

		/// <summary>
		/// Indica se um valor י permitido para este atributo
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool isValidValue(string value)
		{
			return indexValue(value) >= 0;
		}

		/// <summary>
		/// Retorna o םndice de um valor
		/// </summary>
		/// <param name="value">Valor a ser retornado</param>
		/// <returns>O valor do םndice na qual a posiחדo do valor se encontra</returns>
		public int indexValue(string value)
		{
			if (mValues != null)
				return mValues.BinarySearch(value);
			else
				return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (mName != string.Empty)
			{
				return mName;
			}
			else
			{
				return mLabel.ToString();
			}
		}
	}

	/// <summary>
	/// Classe que representarב a arvore de decisדo montada;
	/// </summary>
	public class TreeNode
	{
		private ArrayList mChilds = null;
		private Attribute mAttribute;

		/// <summary>
		/// Inicializa uma nova instגncia de TreeNode
		/// </summary>
		/// <param name="attribute">Atributo ao qual o node estב ligado</param>
		public TreeNode(Attribute attribute)
		{
			if (attribute.values != null)
			{
				mChilds = new ArrayList(attribute.values.Length);
				for (int i = 0; i < attribute.values.Length; i++)
					mChilds.Add(null);
			}
			else
			{
				mChilds = new ArrayList(1);
				mChilds.Add(null);
			}
			mAttribute = attribute;
		}

		/// <summary>
		/// Adiciona um TreeNode filho a este treenode no galho de nome indicicado pelo ValueName
		/// </summary>
		/// <param name="treeNode">TreeNode filho a ser adicionado</param>
		/// <param name="ValueName">Nome do galho onde o treeNode י criado</param>
		public void AddTreeNode(TreeNode treeNode, string ValueName)
		{
			int index = mAttribute.indexValue(ValueName);
			mChilds[index] = treeNode;
		}

		/// <summary>
		/// Retorna o nro total de filhos do nף
		/// </summary>
		public int totalChilds
		{
			get
			{
				return mChilds.Count;
			}
		}

		/// <summary>
		/// Retorna o nף filho de um nף
		/// </summary>
		/// <param name="index">Indice do nף filho</param>
		/// <returns>Um objeto da classe TreeNode representando o nף</returns>
		public TreeNode getChild(int index)
		{
			return (TreeNode)mChilds[index];
		}

		/// <summary>
		/// Atributo que estב conectado ao Nף
		/// </summary>
		public Attribute attribute
		{
			get
			{
				return mAttribute;
			}
		}

		/// <summary>
		/// Retorna o filho de um nף pelo nome do galho que leva atי ele
		/// </summary>
		/// <param name="branchName">Nome do galho</param>
		/// <returns>O nף</returns>
		public TreeNode getChildByBranchName(string branchName)
		{
			int index = mAttribute.indexValue(branchName);
			return (TreeNode)mChilds[index];
		}
	}
	
	/// <summary>
	/// Classe que implementa uma בrvore de Decisדo usando o algoritmo ID3
	/// </summary>
	public class DecisionTreeID3
	{
		private DataTable mSamples;
		private int mTotalPositives = 0;
		private int mTotal = 0;
		private string mTargetAttribute = "result";
		private double mEntropySet = 0.0;

		/// <summary>
		/// Retorna o total de amostras positivas em uma tabela de amostras
		/// </summary>
		/// <param name="samples">DataTable com as amostras</param>
		/// <returns>O nro total de amostras positivas</returns>
		private int countTotalPositives(DataTable samples)
		{
			int result = 0;

			foreach (DataRow aRow in samples.Rows)
			{
				if ((bool)aRow[mTargetAttribute] == true)
					result++;
			}

			return result;
		}

		/// <summary>
		/// Calcula a entropia dada a seguinte fףrmula
		/// -p+log2p+ - p-log2p-
		/// 
		/// onde: p+ י a proporחדo de valores positivos
		///		  p- י a proporחדo de valores negativos
		/// </summary>
		/// <param name="positives">Quantidade de valores positivos</param>
		/// <param name="negatives">Quantidade de valores negativos</param>
		/// <returns>Retorna o valor da Entropia</returns>
		private double calcEntropy(int positives, int negatives)
		{
			int total = positives + negatives;
			double ratioPositive = (double)positives/total;
			double ratioNegative = (double)negatives/total;

			if (ratioPositive != 0)
				ratioPositive = -(ratioPositive) * System.Math.Log(ratioPositive, 2);
			if (ratioNegative != 0)
				ratioNegative = - (ratioNegative) * System.Math.Log(ratioNegative, 2);

			double result =  ratioPositive + ratioNegative;

			return result;
		}

		/// <summary>
		/// Varre tabela de amostras verificando um atributo e se o resultado י positivo ou negativo
		/// </summary>
		/// <param name="samples">DataTable com as amostras</param>
		/// <param name="attribute">Atributo a ser pesquisado</param>
		/// <param name="value">valor permitido para o atributo</param>
		/// <param name="positives">Conterב o nro de todos os atributos com o valor determinado com resultado positivo</param>
		/// <param name="negatives">Conterב o nro de todos os atributos com o valor determinado com resultado negativo</param>
		private void getValuesToAttribute(DataTable samples, Attribute attribute, string value, out int positives, out int negatives)
		{
			positives = 0;
			negatives = 0;

			foreach (DataRow aRow in samples.Rows)
			{
				if (  ((string)aRow[attribute.AttributeName] == value) )
					if ( (bool)aRow[mTargetAttribute] == true) 
						positives++;
					else
						negatives++;
			}		
		}

		/// <summary>
		/// Calcula o ganho de um atributo
		/// </summary>
		/// <param name="attribute">Atributo a ser calculado</param>
		/// <returns>O ganho do atributo</returns>
		private double gain(DataTable samples, Attribute attribute)
		{
			string[] values = attribute.values;
			double sum = 0.0;

			for (int i = 0; i < values.Length; i++)
			{
				int positives, negatives;
				
				positives = negatives = 0;
				
				getValuesToAttribute(samples, attribute, values[i], out positives, out negatives);
				
				double entropy = calcEntropy(positives, negatives);				
				sum += -(double)(positives + negatives)/mTotal * entropy;
			}
			return mEntropySet + sum;
		}

		/// <summary>
		/// Retorna o melhor atributo.
		/// </summary>
		/// <param name="attributes">Um vetor com os atributos</param>
		/// <returns>Retorna o que tiver maior ganho</returns>
		private Attribute getBestAttribute(DataTable samples, Attribute[] attributes)
		{
			double maxGain = 0.0;
			Attribute result = null;

			foreach (Attribute attribute in attributes)
			{
				double aux = gain(samples, attribute);
				if (aux > maxGain)
				{
					maxGain = aux;
					result = attribute;
				}
			}
			return result;
		}

		/// <summary>
		/// Retorna true caso todos os exemplos da amostragem sדo positivos
		/// </summary>
		/// <param name="samples">DataTable com as amostras</param>
		/// <param name="targetAttribute">Atributo (coluna) da tabela a qual serב verificado</param>
		/// <returns>True caso todos os exemplos da amostragem sדo positivos</returns>
		private bool allSamplesPositives(DataTable samples, string targetAttribute)
		{			
			foreach (DataRow row in samples.Rows)
			{
				if ( (bool)row[targetAttribute] == false)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Retorna true caso todos os exemplos da amostragem sדo negativos
		/// </summary>
		/// <param name="samples">DataTable com as amostras</param>
		/// <param name="targetAttribute">Atributo (coluna) da tabela a qual serב verificado</param>
		/// <returns>True caso todos os exemplos da amostragem sדo negativos</returns>
		private bool allSamplesNegatives(DataTable samples, string targetAttribute)
		{
			foreach (DataRow row in samples.Rows)
			{
				if ( (bool)row[targetAttribute] == true)
					return false;
			}

			return true;			
		}

        /// <summary>
        /// Retorna uma lista com todos os valores distintos de uma tabela de amostragem
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <param name="targetAttribute">Atributo (coluna) da tabela a qual serב verificado</param>
        /// <returns>Um ArrayList com os valores distintos</returns>
		private ArrayList getDistinctValues(DataTable samples, string targetAttribute)
		{
			ArrayList distinctValues = new ArrayList(samples.Rows.Count);

			foreach(DataRow row in samples.Rows)
			{
				if (distinctValues.IndexOf(row[targetAttribute]) == -1)
					distinctValues.Add(row[targetAttribute]);
			}

			return distinctValues;
		}

		/// <summary>
		/// Retorna o valor mais comum dentro de uma amostragem
		/// </summary>
		/// <param name="samples">DataTable com as amostras</param>
		/// <param name="targetAttribute">Atributo (coluna) da tabela a qual serב verificado</param>
		/// <returns>Retorna o objeto com maior incidךncia dentro da tabela de amostras</returns>
		private object getMostCommonValue(DataTable samples, string targetAttribute)
		{
			ArrayList distinctValues = getDistinctValues(samples, targetAttribute);
			int[] count = new int[distinctValues.Count];

			foreach(DataRow row in samples.Rows)
			{
				int index = distinctValues.IndexOf(row[targetAttribute]);
				count[index]++;
			}
			
			int MaxIndex = 0;
			int MaxCount = 0;

			for (int i = 0; i < count.Length; i++)
			{
				if (count[i] > MaxCount)
				{
					MaxCount = count[i];
					MaxIndex = i;
				}
			}

			return distinctValues[MaxIndex];
		}

		/// <summary>
		/// Monta uma בrvore de decisדo baseado nas amostragens apresentadas
		/// </summary>
		/// <param name="samples">Tabela com as amostragens que serדo apresentadas para a montagem da בrvore</param>
		/// <param name="targetAttribute">Nome da coluna da tabela que possue o valor true ou false para 
		/// validar ou nדo uma amostragem</param>
		/// <returns>A raiz da בrvore de decisדo montada</returns></returns?>
		private TreeNode internalMountTree(DataTable samples, string targetAttribute, Attribute[] attributes)
		{
			if (allSamplesPositives(samples, targetAttribute) == true)
				return new TreeNode(new Attribute(true));
			
			if (allSamplesNegatives(samples, targetAttribute) == true)
				return new TreeNode(new Attribute(false));

			if (attributes.Length == 0)
				return new TreeNode(new Attribute(getMostCommonValue(samples, targetAttribute)));			
		
			mTotal = samples.Rows.Count;
			mTargetAttribute = targetAttribute;
			mTotalPositives = countTotalPositives(samples);

			mEntropySet = calcEntropy(mTotalPositives, mTotal - mTotalPositives);
			
			Attribute bestAttribute = getBestAttribute(samples, attributes); 

			TreeNode root = new TreeNode(bestAttribute);
			
			DataTable aSample = samples.Clone();			
			
			foreach(string value in bestAttribute.values)
			{				
				// Seleciona todas os elementos com o valor deste atributo				
				aSample.Rows.Clear();

				DataRow[] rows = samples.Select(bestAttribute.AttributeName + " = " + "'"  + value + "'");
			
				foreach(DataRow row in rows)
				{					
					aSample.Rows.Add(row.ItemArray);
				}				
				// Seleciona todas os elementos com o valor deste atributo				

				// Cria uma nova lista de atributos menos o atributo corrente que י o melhor atributo				
				ArrayList aAttributes = new ArrayList(attributes.Length - 1);
				for(int i = 0; i < attributes.Length; i++)
				{
					if (attributes[i].AttributeName != bestAttribute.AttributeName)
						aAttributes.Add(attributes[i]);
				}
				// Cria uma nova lista de atributos menos o atributo corrente que י o melhor atributo

				if (aSample.Rows.Count == 0)
				{
					return new TreeNode(new Attribute(getMostCommonValue(aSample, targetAttribute)));
				}
				else
				{				
					DecisionTreeID3 dc3 = new DecisionTreeID3();
					TreeNode ChildNode =  dc3.mountTree(aSample, targetAttribute, (Attribute[])aAttributes.ToArray(typeof(Attribute)));
					root.AddTreeNode(ChildNode, value);
				}
			}

			return root;
		}


		/// <summary>
		/// Monta uma בrvore de decisדo baseado nas amostragens apresentadas
		/// </summary>
		/// <param name="samples">Tabela com as amostragens que serדo apresentadas para a montagem da בrvore</param>
		/// <param name="targetAttribute">Nome da coluna da tabela que possue o valor true ou false para 
		/// validar ou nדo uma amostragem</param>
		/// <returns>A raiz da בrvore de decisדo montada</returns></returns?>
		public TreeNode mountTree(DataTable samples, string targetAttribute, Attribute[] attributes)
		{
			mSamples = samples;
			return internalMountTree(mSamples, targetAttribute, attributes);
		}
	}

	/// <summary>
	/// Classe que exemplifica a utilizaחדo do ID3
	/// </summary>
	class ID3Sample
	{

		public static void printNode(TreeNode root, string tabs)
		{
			Console.WriteLine(tabs + '|' + root.attribute + '|');
			
			if (root.attribute.values != null)
			{
				for (int i = 0; i < root.attribute.values.Length; i++)
				{
					Console.WriteLine(tabs + "\t" + "<" + root.attribute.values[i] + ">");
					TreeNode childNode = root.getChildByBranchName(root.attribute.values[i]);
					printNode(childNode, "\t" + tabs);
				}
			}
		}


		static DataTable getDataTable()
		{
			DataTable result = new DataTable("samples");
			DataColumn column = result.Columns.Add("ceu");
			column.DataType = typeof(string);
			
			column = result.Columns.Add("temperatura");
			column.DataType = typeof(string);

			column = result.Columns.Add("humidade");
			column.DataType = typeof(string);

			column = result.Columns.Add("vento");
			column.DataType = typeof(string);

			column = result.Columns.Add("result");
			column.DataType = typeof(bool);

			result.Rows.Add(new object[] {"sol", "alta", "alta", "nao", false}); //D1 sol alta alta nדo N
			result.Rows.Add(new object[] {"sol", "alta", "alta", "sim", false}); //D2 sol alta alta sim N
			result.Rows.Add(new object[] {"nublado", "alta", "alta", "nao", true}); //D3 nebulado alta alta nדo P
			result.Rows.Add(new object[] {"chuva", "alta", "alta", "nao", true}); //D4 chuva alta alta nדo P
			result.Rows.Add(new object[] {"chuva", "baixa", "normal", "nao", true}); //D5 chuva baixa normal nדo P
			result.Rows.Add(new object[] {"chuva", "baixa", "normal", "sim", false}); //D6 chuva baixa normal sim N
			result.Rows.Add(new object[] {"nublado", "baixa", "normal", "sim", true}); //D7 nebulado baixa normal sim P
			result.Rows.Add(new object[] {"sol", "suave", "alta", "nao", false}); //D8 sol suave alta nדo N
			result.Rows.Add(new object[] {"sol", "baixa", "normal", "nao", true}); //D9 sol baixa normal nדo P
			result.Rows.Add(new object[] {"chuva", "suave", "normal", "nao", true}); //D10 chuva suave normal nדo P
			result.Rows.Add(new object[] {"sol", "suave", "normal", "nao", true}); //D11 sol suave normal sim P
			result.Rows.Add(new object[] {"nublado", "suave", "alta", "sim", true}); //D12 nebulado suave alta sim P
			result.Rows.Add(new object[] {"nublado", "alta", "normal", "nao", true}); //D13 nebulado alta normal nדo P
			result.Rows.Add(new object[] {"chuva", "suave", "alta", "sim", false}); //D14 chuva suave alta sim N

			return result;
			
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 
		[STAThread]
		static void Main(string[] args)
		{
			Attribute ceu = new Attribute("ceu", new string[] {"sol", "nublado", "chuva"});
			Attribute temperatura = new Attribute("temperatura", new string[] {"alta", "baixa", "suave"});
			Attribute humidade = new Attribute("humidade", new string[] {"alta", "normal"});
			Attribute vento = new Attribute("vento", new string[] {"sim", "nao"});

			Attribute[] attributes = new Attribute[] {ceu, temperatura, humidade, vento};
			
			DataTable samples = getDataTable();			

			DecisionTreeID3 id3 = new DecisionTreeID3();
			TreeNode root = id3.mountTree(samples, "result", attributes);

			printNode(root, "");

		}
	}
}
