   M S B A M L   `   `   `  ���� D  @XKeyboard, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:XKeyboard.UIXKeyboard.UI      XKeyboard.UI.frmEdit   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:XKeyboard.UI       � Title$  XKeyboard - Edit��$��416.515��$��481.955��   darkButtonBackClick#��[  .��� .+��� -    ��Name$ btnSave��$
��Save��$
��Left=�$��
10,0,0,4.8q�$��Bottom=�$��75�� � ToolTip$ Save the current font set���� -   $ 	btnSaveAs��$��Save As��$
��Left=�$��
90,0,0,4.8q�$��75��$+ %Export current font set to a new file��$��19��$��Bottom=��� -    ��Name$ label��$��Name:��$
��Left=�$	��Top=����	#FFF6EBEB�� -    ��Name$ txtName��$��23��$��79,2,10.2,0q� �� TextWrapping$
 Wrap=�$	��Top=�$��Center=�$$ The short name of the font set���� -   $ 
label_Copy��$��Description:��$
��Left=�$	��Top=�$��0,28,0,0q����	#FFF6EBEB�� -   $ txtDescription��$��23��$��79,30,10.2,0q�$
 Wrap=�$	��Top=�$��Center=�$' !Short description of the font set���� -   $ label_Copy1��$��Author:��$
��Left=�$	��Top=�$��2,58,0,0q� J� RenderTransformOrigin$ 0.61,-0.189&����	#FFF6EBEB�� -   $ 	txtAuthor��$��23��$��79,60,10.2,0q�$
 Wrap=�$	��Top=�$��Center=�$' !Short description of the font set���� -	    ��Name$ listView��$��10,180,10.2,29.2q� �� SelectionMode$ Single=�	 �� View	 �� 
 �� AllowsColumnReorder
 .    MyColumnHeader! �� ColumnHeaderContainerStyle# �  .P��� $
��CODE�� �� Width$ 50���� $	��KEY��$ 60���� $��VALUE��$	 200������ .R��  � ColumnDefinitions ��  ��Name$ colCode��$��50 ��� $ colKey��$��60 ��� $ colValue��$	��200 �.+��� $��0��$��Center=����� *	KeyCode+�� $��1��$��Center=����� *Key+�� $��Stretch=�$��2������ *TargetValue+ �� Mode$ TwoWay=��� -
   $ label_Copy3��$��	Autofill:��$
��Left=�$	��Top=�$��2,89,0,0q�$ 0.61,-0.189&����	#FFF6EBEB�� -   $ txtAutofill��$��23��$��79,90,90.2,0q�$
 Wrap=�$	��Top=�$��Center=�$O IPaste the custom font in order of 0-9A-Za-z to automatically set values. ���� -   $ btnAutoFill��$
��Fill��$��0,92,10.2,0q�$	��Top=�$��Right=�$��75���� -   $ label_Copy4��$��Add Key:��$
��Left=�$	��Top=�$��	2,119,0,0q�$ 0.61,-0.189&����	#FFF6EBEB�� -   $ btnAddNewKey��$! Add missing key to the list��$	��Add��$��0,123,10.2,0q�$	��Top=�$��Right=�$��75���� -   $ btnRemoveSelectedKey��$( "Remove selected key from the font.��$��Remove��$��0,153,10.2,0q�$	��Top=�$��Right=�$��75���� -   $ 	txtAddKey��$��23��$��
79,121,0,0q�$
 Wrap=�$	��Top=�$��Center=�$4 .The key char to add in fontset for replacement��$
��Left=�$	��148���� -   $ txtAddKeyValue��$��23��$��0,121,90.2,0q�$
 Wrap=�$	��Top=�$��Center=�$2 ,The target key value to replace the key with��$��Right=�$	��148��