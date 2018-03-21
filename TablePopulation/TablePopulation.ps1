param (
  [Parameter(Mandatory = $true)]
  [string]$StorageAccountName = "wineinvstordev",
  
  [Parameter(Mandatory = $true)]
  [string]$ResourceGroupName = "WineShop",
  
  [string]$TableName = "inventory",
  
  [string]$SubscriptionId,

  [switch]$CD = $false
)

function Insert-Row($table, [String]$partitionKey, [String]$rowKey, [int]$wineInStock, [string]$wineInfo, [string] $wineName, [string] $winePicture, [double] $winePrice)
{
  $entity = New-Object "Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" $partitionKey, $rowKey
  $entity.Properties.Add("WineInStock", $wineInStock)
  $entity.Properties.Add("WineInfo", $wineInfo)
  $entity.Properties.Add("WineName", $wineName)
  $entity.Properties.Add("WinePicture", $winePicture)
  $entity.Properties.Add("WinePrice", $winePrice)
  try {
    $result = $table.Execute([Microsoft.WindowsAzure.Storage.Table.TableOperation]::Insert($entity))
  } catch {
    Write-Host "Row" $partitionKey "|" $rowKey "not inserted, it may already exist" -ForegroundColor Yellow
  }
}

if(!$CD) {
    # logs in
    Login-AzureRmAccount

    if(($SubscriptionId -eq "") -or ($SubscriptionId -eq $null)) {
        Write-Host "You are not running in CD mode, please specify your subscription ID"
        $SubscriptionId = Read-Host
    }

    # select subscription
    Select-AzureRmSubscription -SubscriptionId $SubscriptionId
}

$context = (Get-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageAccountName).Context
$connectionString = $context.ConnectionString


# check if table exists, if not create it
$table = Get-AzureStorageTable $TableName -Context $context -ErrorAction Ignore
if ($table -eq $null)
{
    New-AzureStorageTable $TableName -Context $context
}

$cloudAccount = [Microsoft.WindowsAzure.Storage.CloudStorageAccount]::Parse($connectionString)
$tableClient = $cloudAccount.CreateCloudTableClient()
$table = $tableClient.GetTableReference($TableName)

Insert-Row -table $table -partitionKey "Red" -rowKey "0" -wineInStock 10 -winePrice 13.95 -wineName "Chateauneuf Du Pape Les Courlandes 75cl" -winePicture "https://wine-searcher1.freetls.fastly.net/images/labels/91/53/jacques-charlet-chateauneuf-du-pape-les-clefs-d-or-rhone-france-10679153.jpg" -wineInfo "The village of Chateauneuf du Pape in the southern Rhone valley is named after the new ch�teau built by the Popes as a summer residence in the 14th century. Its vineyards, first planted in Roman times, and covered with rolled pebbles, are renowned for their rich, ripe, generous wines"
Insert-Row -table $table -partitionKey "Red" -rowKey "1" -wineInStock 7 -winePrice 12.90 -wineName "Journeys End Bluegum Merlot 75cl" -winePicture "http://www.buywine.co.za/wp-content/uploads/2015/01/Journeys-End.png" -wineInfo "Mineral rich, granite soils, cool coastal breezes and long sunny days have played their part in delivering healthy concentrated berries, handpicked and carefully selected to provide the perfect foundation for this wine. Following fermentation in open fermenters, this Merlot was subsequently allowed to age for 18 months in 3001 French oak barrel for added complexity. It is an elegant, soft, fruit driven wine that will drink well now with added cellaring potential of 5-10 years."
Insert-Row -table $table -partitionKey "Red" -rowKey "2" -wineInStock 1 -winePrice 11.95 -wineName "Barossa Valley Estate Cabernet Sauvignon 75cl" -winePicture "https://shop.countdown.co.nz/Content/ProductImages/large/9311347003113.jpg" -wineInfo "Our wines capture the distinctive elegance, finesse and vibrant fruit flavours of one of the world's most celebrated wine regions. Welcome to the Barossa Valley."
Insert-Row -table $table -partitionKey "Rose" -rowKey "0" -wineInStock 12 -winePrice 9.97 -wineName "Jacob's Creek Sparkling Rose 75cl" -winePicture "https://www.ocado.com/productImages/346/34694011_0_640x640.jpg?identifier=49592f6066b65a9ba1b5ea2144064004" -wineInfo "This sweet sparkling rose is soft and fruity with flavours of lusciously ripe strawberries. Serve well-chilled with summer berry desserts."
Insert-Row -table $table -partitionKey "White" -rowKey "0" -wineInStock 18 -winePrice 6.99 -wineName "Antico Palazzo Chardonnay 75cl" -winePicture "http://www.sainsburys.co.uk/wcsstore7.23.1.52/ExtendedSitesCatalogAssetStore/images/catalog/productImages/88/8007890004288/8007890004288_L.jpeg" -wineInfo "This wine marries the fresh, zesty notes of Chardonnay with the delicate floral aromas of Pinot Grigio. It makes a great aperitif but is also delicious paired with seafood and salads."
Insert-Row -table $table -partitionKey "White" -rowKey "1" -wineInStock 40 -winePrice 8.50 -wineName "Dark Horse Chardonnay 75cl" -winePicture "http://www.sainsburys.co.uk/wcsstore7.23.1.52/ExtendedSitesCatalogAssetStore/images/catalog/productImages/19/0085000020319/0085000020319_L.jpeg" -wineInfo "A bold with big personality, this Chardonnay showcase bright flavours of apple, pear, caramel and a creamy, full-bodied finish.  Dark Horse winemaker, Beth Liston, believes that fortune flavours the bold. Her pioneering approach to viticulture and winemaking champions originality and above all, taste."