#reads from external solution which is a stand alone DLL.
#must be references in C# and "Using"
#from IR2 import *

#if loading from our own DLL use this
from IronPythonDLL import *
print 'Rule1'

if saleBasket.Total > 100:
	discount = saleBasket.Total * -0.1
	saleBasket.Lines.Add(Line(ProductName="Dummy", ProductPrice=discount, Quantity=1, Amount=discount))
	print 'discount given: ' + (discount * -1).ToString()