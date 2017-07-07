#reads from external solution which is a stand alone DLL.
#must be references in C# and "Using"
#from IR2 import *

#if loading from our own DLL use this
from IronPythonDLL import *
print 'Rule2'

for line in saleBasket.Lines:
	
	if line.ProductName == 'Prod1':
		
		discount = line.Amount * 0.2
		line.Amount = line.Amount - discount
		print 'discount given: ' + discount.ToString()
	
	if line.Quantity >= 10:
		
		line.Amount = line.Amount - line.ProductPrice
		print 'discount given: ' + line.ProductPrice.ToString()