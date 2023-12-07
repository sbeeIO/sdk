<?php
/*
WHAT IS SBEE?
SBEE EXCHANGE ENGINE (SBEE) IS AN INTERMEDIARY SOFTWARE THAT ACTS AS A LINK CONNECTING CRYPTOCURRENCY EXCHANGES AND APPLICATIONS.
BY INTEGRATING WITH SBEE API SERVICES, APPLICATIONS CAN ESTABLISH CONNECTIONS TO MULTIPLE CRYPTOCURRENCY EXCHANGES SEAMLESSLY. 
THIS INTEGRATION PROVIDES USERS WITH THE FLEXIBILITY TO TRADE ON VARIOUS EXCHANGES, ACCESS LIVE DATA, AND PERFORM DIVERSE TRANSACTIONS ALL FROM A CENTRALIZED PLATFORM. 
TO UTILIZE THIS LIBRARY, YOU NEED TO GENERATE A FREE BEARER TOKEN FROM WWW.SBEE.IO. 
BY ENTERING THE NAME OF THE EXCHANGE YOU WANT TO OPERATE ON AND PROVIDING YOUR TRANSACTION DETAILS, 
YOU CAN EASILY AND QUICKLY EXECUTE YOUR TRANSACTIONS. FOR DETAILED INFORMATION ABOUT THE SERVICE, YOU CAN ACCESS DOC.SBEE.IO.
*/

class SbeeRest{
    private $baseURL; //'https://api.sbee.io/api'
    private $auth;	  

    public function __construct($baseURL, $auth) {
        $this->baseURL = $baseURL;
        $this->auth = $auth;
    }
	/*
	Server connection functionality 
	*/
	private function makeRequest($url, $method, $headers = [], $data = null) {
		if (!in_array($method, ['GET', 'POST'])) { throw new Exception('Invalid HTTP method.'); }
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_URL, $url);
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);
		if ($method === 'POST' && $data !== null) { curl_setopt($ch, CURLOPT_POSTFIELDS, $data); }
		curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
		curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 2);
		curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);
		curl_setopt($ch, CURLOPT_SAFE_UPLOAD, true);
		curl_setopt($ch, CURLOPT_TIMEOUT, 60); // Set the timeout duration to 15 seconds.
		$response = curl_exec($ch);
		if ($response === false) { throw new Exception('Curl error: ' . curl_error($ch)); }
		curl_close($ch);
		return $response;
	}
	/*
	https://doc.sbee.io/api/get-system-time
	Get System Time
	Exchange server time information
	@params $Exchange='Binance'
	*/
    public function SystemTime($Exchange) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/SystemTime';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth];
        try {
            $response = $this->makeRequest($url, 'GET', $headers);
            return json_decode($response, true);
        } catch (Exception $e) { 
            return ['ERROR' => $e];
        }
    }
	/*
	https://doc.sbee.io/api/public-endpoints/spot/recent-trades
	Recent Trades
	The purpose of using the "Recent Trades" method in cryptocurrency futures trading is to view the recent trades that have taken place on a specific futures contract.
	Past fulfilled buy and sell orders
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $limit='20' 
	*/
    public function RecentTrades($Exchange, $Trade, $symbol, $depth) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/RecentTrades?symbol=' . urlencode($symbol) . '&depth=' . $depth;
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth];
		 try {
            $response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
        } catch (Exception $e) { 
            return ['ERROR' => $e];
        }
    }
	/*
	https://doc.sbee.io/api/public-endpoints/spot/currencies
	Currencies
	Gets all tradable pairs and their quantity or price scales.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	*/
	public function Currencies($Exchange,$Trade) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/Currencies';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth];
		try {
            $response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
        } catch (Exception $e) { 
            return ['ERROR' => $e];
        }
    }
	/*
	https://doc.sbee.io/api/spot/trading-balances
	Reads wallet information.Gets all cash balances.
	Withdraws all coins when symbol information is left blank
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='USDT'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
    public function TradingBalances($Exchange, $Trade, $symbol, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/TradingBalances';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = json_encode([
            'symbol' => $symbol,
            'apiKey' => $apiKey,
            'apiSecret' => $apiSecret,
            'apiPass' => $apiPass
        ]);
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
        } catch (Exception $e) { 
            return ['ERROR' => $e];
        } 
    }
	/*
	https://doc.sbee.io/api/spot/order-history
	My buy and sell orders
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $state='NEW,ALL,FILLED,CANCELED'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
	public function OrderHistory($Exchange, $Trade, $symbol, $state, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/OrderHistory';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = json_encode([
            'symbol' => $symbol,
            'state' => $state,
            'apiKey' => $apiKey,
            'apiSecret' => $apiSecret,
            'apiPass' => $apiPass
        ]);
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		} 
    }
	/*
	https://doc.sbee.io/api/public-endpoints/spot/kline
	KLine
	Kline/candlestick bars for a symbol. The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
	Pulls historical candle information
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $interval='1m, 5m, 15m, 30m, 1h, 4h, 1d, 1M'
	@param $startTime='1689170400000'
	@param $startTime='1689970459999'
	@param $endTime='1603152000'
	@param $limit='10'
	*/
	public function KLine($Exchange, $Trade, $symbol, $interval, $startTime, $endTime, $limit) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/KLine?symbol=' . urlencode($symbol) . '&interval=' . urlencode($interval) . '&startTime=' . urlencode($startTime) . '&endTime=' . urlencode($endTime) . '&limit=' . $limit;
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth]; 
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/kline-formation
	Kline Formation
	It is a technical analysis tool used in cryptocurrency trading.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $interval='1m, 5m, 15m, 30m, 1h, 4h, 1d, 1M'
	@param $startTime='1689170400000'
	@param $startTime='1689970459999'
	@param $endTime='1603152000'
	@param $limit='100'
	$formations = '[ 
					{
						"Formation": "MAX",
						"TimePeriod": 30,
						"Source": "close"
					}, 
					{
						"Formation": "DX",
						"TimePeriod": 14
					},
					{
						"Formation": "MACD",
						"FastPeriod": 12,
						"SlowPeriod": 26,
						"SignalPeriod": 9,
						"Source": "close"
					} 
				]'; 
	*/
	public function KlineFormation($Exchange,$Trade,$symbol,$interval,$limit,$formations,$startTime=null,$endTime=null) {
		if(empty($startTime) or empty($endTime)){ $startTime='null'; $endTime='null'; }  
		$data='{
				  "symbol": "'.$symbol.'",
				  "interval": "'.$interval.'",
				  "limit":'.$limit.', 		
				  "startTime":'.$startTime.', 
			 	  "endTime":'.$endTime.', 
				  "formations": '.$formations.'
				}';
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/KlineFormation';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	Order Book
	Gets an instant list of all open orders for a product.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $depth='20'
	*/
	public function OrderBook($Exchange, $Trade, $symbol, $depth) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/OrderBook?symbol=' . urlencode($symbol) . '&depth=' . $depth;
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth]; 
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	Tickers
	Gets snapshot information about the latest transaction price, best bid/ask and 24h transaction volume.
	@param 'Binance'
	@param $Trade ='Spot' //Futures
	@param 'BTC-USDT'
	*/
    public function Tickers($Exchange,  $Trade, $symbol) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/Tickers?symbol=' . urlencode($symbol);
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth];
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/limit-order
	Enters a buy or sell order
	@param $Exchange='Binance' 
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $ClientOrderId='ID3231'
	@param $price='16000'
	@param $quoteQuantity='0'
	@param $baseQuantity='0.005'
	@param $side='BUY'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
	public function PlaceLimitOrder($Exchange, $Trade,  $symbol, $ClientOrderId, $price, $quoteQuantity, $baseQuantity, $side, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceLimitOrder';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass,
            "symbol" => $symbol,
            "ClientOrderId" => $ClientOrderId,
            "price" => $price,
            "quoteQuantity" => $quoteQuantity,
            "baseQuantity" => $baseQuantity,
            "side" => $side
        ];
        $dataJson = json_encode($data); 
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		} 
    }
	/*
	https://doc.sbee.io/api/spot/market-order
	Executes a buy or sell order at market price
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT';
	@param $ClientOrderId='ID326511'
	@param $price='26000'
	@param $quoteQuantity='15'
	@param $baseQuantity='0'
	@param $side='BUY'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/ 
	public function PlaceMarketOrder($Exchange, $Trade, $symbol, $ClientOrderId, $price, $quoteQuantity, $baseQuantity,$leverage=0,$contract=0, $side, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceMarketOrder';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass,
            "symbol" => $symbol,
            "ClientOrderId" => $ClientOrderId,
			"price" => $price,
            "quoteQuantity" => $quoteQuantity,
            "baseQuantity" => $baseQuantity,
			"leverage" => $leverage,
			"contract" => $contract,
            "side" => $side
        ];
        $dataJson = json_encode($data); 
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/limit-stop-loss-order
	Enters a buy or sell stop loss limit order
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $quantity='0.0005'
	@param $ClientOrderId='ID653'
	@param $stopPrice='28000'
	@param $orderPrice='0'
	@param $price='27500'
	@param $trailingDelta='0'
	@param $side='BUY'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
    public function PlaceLimitStopLossOrder($Exchange, $Trade, $symbol, $quantity, $ClientOrderId, $stopPrice, $orderPrice, $price, $trailingDelta, $side, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceLimitStopLossOrder';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass,
            "symbol" => $symbol,
            "quantity" => $quantity,
            "ClientOrderId" => $ClientOrderId,
            "stopPrice" => $stopPrice,
            "orderPrice" => $orderPrice,
            "price" => $price,
            "trailingDelta" => $trailingDelta,
            "side" => $side
        ];
        $dataJson = json_encode($data);
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		} 
    }
	/*
	https://doc.sbee.io/api/spot/limit-take-profit-order
	Limit Take Profit Order A type of limit order that specifies the exact price at which to close out an open position for a profit.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $quantity='0.005'
	@param $ClientOrderId='ID653323' 
	@param $stopPrice='25000'
	@param $orderPrice='22000'
	@param $price='20000'
	@param $trailingDelta='0'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
    public function PlaceLimitTakeProfitOrder($Exchange, $Trade,  $symbol, $quantity, $ClientOrderId, $stopPrice, $orderPrice, $price, $trailingDelta, $side, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceLimitTakeProfitOrder';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass,
            "symbol" => $symbol,
            "quantity" => $quantity,
            "ClientOrderId" => $ClientOrderId,
            "stopPrice" => $stopPrice,
            "orderPrice" => $orderPrice,
            "price" => $price,
            "trailingDelta" => $trailingDelta,
            "side" => $side 
        ];
        $dataJson = json_encode($data); 
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
 
	/*
	https://doc.sbee.io/api/private-endpoints/futures/set-leverage
	Set Leverage
	Allows the leverage value to be defined.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $leverage='5'
	*/
    public function SetLeverage($Exchange, $Trade,  $symbol,$leverage, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/SetLeverage';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass,
            "symbol" => $symbol,
			"leverage" => $leverage,
        ];
        $dataJson = json_encode($data); 
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/cancel-order
	Cancels the entered buy or sell order
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $orderId='43523123123'
	@param $clientOrderId='ID3421'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
    public function CancelOrder($Exchange, $Trade, $symbol, $orderId, $clientOrderId, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/CancelOrder';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass,
            "symbol" => $symbol,
            "orderId" => strval($orderId),
            "clientOrderId" => strval($clientOrderId)
        ];
        $dataJson = json_encode($data);
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		} 
    }
	
	/*
	https://doc.sbee.io/api/spot/cancel-order-by-symbol
	Cancel Order By Symbol
	Cancel the buy or sell order entered with the symbol name.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $orderId='43523123123'
	@param $clientOrderId='ID3421'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
	public function CancelOrdersBySembol($Exchange, $Trade,  $symbol, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/CancelOrdersBySembol';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
			"symbol" => $symbol,
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass
             
        ];
        $dataJson = json_encode($data);
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $dataJson);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		} 
    }
	/*
	https://doc.sbee.io/api/spot/batch-processes/cancel-batch-orders
	Cancels a buy or sell bulk order entered in the same wallet 
	$orders = json_encode([
					"apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj0ibhFP7uX80tVWDpkFTPOLAx4DIM",
					"apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOaCV91QVzd8MbgQJMcvkXju3Qey5VRwfge",
					"apiPass" => "",
					"orders" => [
						[
							"symbol" => "BTC-USDT",
							"clientOrderId" => "ID123",
							 "orderId" => "ID124", 
						],[
							"symbol" => "BTC-USDT",
							"clientOrderId" => "ID126",
							 "orderId" => "ID127", 
						],
					]
				]);
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
	public function CancelBatchOrders($Exchange, $Trade,  $orders, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/CancelBatchOrders';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json']; 
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $orders);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
	/*
	https://doc.sbee.io/api/spot/batch-processes/cancel-batch-orders-for-people
	Bulk buy or sell order entered from different wallets cancels
	$orders='[
			{
			"symbol": "BTC-USDT",
			"orderId": "",
			"clientOrderId": "ID901",
			"apiKey": "ascvaxvv8K5oDjVmZCffg3cNjNS4Ue19VTKJ.................",
			"apiSecret": "asdasXFtldx3VSBfl356c8F5f8swAH2QY................",
			"apiPass": "string"
		  },{
			"symbol": "BTC-USDT",
			"orderId": "",
			"clientOrderId": "ID902",
			"apiKey": "wdfdslvg8K5oDjVmZCffg3cNjNS4Ue19VTKJ................",
			"apiSecret": "xcvcxvFtldx3VSBfl356c8F5f8swAH2QY................",
			"apiPass": "string"
		  },{
			"symbol": "BTC-USDT",
			"orderId": "",
			"clientOrderId": "ID903",
			"apiKey": "vbcvldv8K5oDjVmZCffg3cNjNS4Ue19VTKJu................"",
			"apiSecret": "cvbwrFtldx3VSBfl356c8F5f8swAH2QYo................",
			"apiPass": "string"
		  }
		]';
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures

	*/
    public function CancelBatchOrdersForPeople($Exchange, $Trade,  $orders) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/CancelBatchOrdersForPeople';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $orders);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/batch-processes/batch-market-orders
	Batch Market Orders
	Open more than once market transactions from a single account.
	$orders = json_encode([
					"apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj.......",
					"apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOH........",
					"apiPass" => "OKX",
					"orders" => [
						[
							"symbol" => "BTC-USDT",
							"quoteQuantity" => 1,
							"baseQuantity" => 0,
							"clientOrderId" => "ID123",
							"side" => "buy", 
						],
						[
							"symbol" => "BTC-USDT",
							"quoteQuantity" => 1,
							"baseQuantity" => 0,
							"clientOrderId" => "ID124",
							"side" => "buy",
						],
					]
				]);

	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	*/
    public function PlaceBatchMarketOrders($Exchange, $Trade, $orders) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceBatchMarketOrders';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $orders);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }	

	/*
	https://doc.sbee.io/api/spot/trading-balances-for-people
	Trading Balances For People
	Gets all cash balances for more than one account.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	$orders = json_encode(
				[
				  {
				  "symbol": "BTC-USDT", 
				  "apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj.......",
				  "apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOH........",
				  "apiPass" => "",
				  },
				  {
				  "symbol": "XRP-USDT", 
				  "apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj.......",
				  "apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOH........",
				  "apiPass" => "",
				  }
				]
			);
		
	*/
	public function TradingBalancesForPeople($Exchange,  $Trade, $orders) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/TradingBalancesForPeople';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $orders);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	Cancels the entered buy and sell orders according to the symbol.
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $symbol='BTC-USDT'
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
    public function CancelOrdersBySymbol($Exchange,  $Trade, $symbol, $apiKey, $apiSecret, $apiPass) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/CancelOrdersBySymbol';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = [
            "symbol" => $symbol,
            "apiKey" => $apiKey,
            "apiSecret" => $apiSecret,
            "apiPass" => $apiPass
        ];  
		try {
			$response = $this->makeRequest($url, 'POST', $headers, json_encode($data));
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/batch-processes/batch-limit-orders
	Enters bulk limit buy and sell orders from the same wallet
	$orders = json_encode([
					"apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj0ibhFP7uX80tVWDpkFTPOLAx4DIM",
					"apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOaCV91QVzd8MbgQJMcvkXju3Qey5VRwfge",
					"apiPass" => "",
					"orders" => [
						[
							"symbol" => "BTC-USDT",
							"clientOrderId" => rand(10000,99999),
							"price" => 20000,
							"quoteQuantity" => 0,
							"baseQuantity" => 0.005,
							"side" => "BUY"
						],[
							"symbol" => "BTC-USDT",
							"clientOrderId" => rand(10000,99999),
							"price" => 20000,
							"quoteQuantity" => 0,
							"baseQuantity" => 0.005,
							"side" => "BUY"
						],
					]
				]);
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param $apiKey='Key...'
	@param $apiSecret='Secret...'
	@param $apiPass='Pass..'
	*/
    public function PlaceBatchLimitOrders($Exchange, $Trade, $orders) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceBatchLimitOrders';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json']; 
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $orders);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/limit-order-for-people
	Enters bulk limit buy and sell orders from different wallets
	$orders='[ 
		 {
			"apiKey": "vbcvldv8K5oDjVmZCffg3cNjNS4Ue19VTKJu................"",
			"apiSecret": "cvbwrFtldx3VSBfl356c8F5f8swAH2QYo................"",
			"apiPass": "string",
			"side": "buy",
			"price": 10000,
			"baseQuantity":0.001,
			"quoteQuantity": 0,
			"cliOrId": "UD01",
			"symbol": "BTC-USDT"
		  },{
			"apiKey": "ascvaxvv8K5oDjVmZCffg3cNjNS4Ue19VTKJ.................",
			"apiSecret": "asdasXFtldx3VSBfl356c8F5f8swAH2QY................"",
			"apiPass": "string",
			"side": "buy",
			"price": 10000,
			"baseQuantity":0.001,
			"quoteQuantity": 0,
			"cliOrId": "UD02",
			"symbol": "BTC-USDT"
		  }
		]';
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	@param '$orders'
	*/
    public function PlaceLimitOrderForPeople($Exchange, $Trade, $orders) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceLimitOrderForPeople';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $orders);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR_PlaceLimitOrderForPeople' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/spot/market-order-for-people
	Enters bulk market buy and sell orders from different wallets
	$orders ='[
		  {
			"symbol": "BTC-USDT",
			"quoteQuantity": 11,
			"baseQuantity": 0,
			"ClientOrderId": "UD01",
			"side": "BUY",
			"apiKey": "ascvaxvv8K5oDjVmZCffg3cNjNS4Ue19VTKJ.................",
			"apiSecret": "asdasXFtldx3VSBfl356c8F5f8swAH2QY................"",
			"apiPass": "string"
		  }, { 
			"symbol": "BTC-USDT",
			"quoteQuantity": 11,
			"baseQuantity": 0,
			"ClientOrderId": "UD01",
			"side": "BUY",
			"apiKey": "vbcvldv8K5oDjVmZCffg3cNjNS4Ue19VTKJu................"",
			"apiSecret": "cvbwrFtldx3VSBfl356c8F5f8swAH2QYo................"",
			"apiPass": "string"
		  }
		]';
	@param $Exchange='Binance'
	@param $Trade ='Spot' //Futures
	*/
   public function PlaceMarketOrderForPeople($Exchange, $Trade, $orders) {
        $url = $this->baseURL . '/Crypto/' . $Exchange . '/' . $Trade . '/PlaceMarketOrderForPeople';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
        $data = $orders;
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/info-markets
	It provides information about the owned stock exchange and the service endpoints used in the exchange.
	*/
	public function Markets() {
        $url = $this->baseURL.'/Crypto/Info/Markets';
		$headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth];
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
	/*
	https://doc.sbee.io/api/fintech
	It adjusts the value of currencies relative to each other.
	*/
	public function MoneyPairValues() {
        $url = $this->baseURL.'/Fintech/MoneyPairValues';
		$headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth];
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
	/*
	https://doc.sbee.io/api/multi-market/order-book
	Order Book
	Provides the depth of buy and sell orders (quantity and price levels) for a specific asset across multiple exchanges.
	@param $Trade ='Spot' //Futures
	$data = '{
	  "symbol": "ADA-USDT",
	  "depth": 50,
	  "precision": 3,
	  "exchanges": [
		"Binance","Binance","Kraken","KuCoin","Bybit","OKX","GateIO","Mexc"
	  ]
	}'; 
	*/ 
	public function MultiOrderBook($Trade,$data) {
        $url = $this->baseURL . '/Crypto/MultiMarket/' . $Trade . '/OrderBook';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
	/*
	https://doc.sbee.io/api/multi-market/recent-trades
	Recent Trades
	Shows recent trade transactions for a specific asset.
	@param $Trade ='Spot' //Futures
	$data = '{
	  "symbol": "BTC-USDT",
	  "depth": 50,
	  "exchanges": [
		"Binance","Binance","Kraken","KuCoin","Bybit","OKX","GateIO","Mexc"
	  ]
	}'; 
	*/ 
	 public function MultiRecentTrades($Trade,$data) {
        $url = $this->baseURL . '/Crypto/MultiMarket/' . $Trade . '/RecentTrades';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	/*
	https://doc.sbee.io/api/multi-market/stepped-order-book
	Stepped Order Book
	Provides a stepped order book, displaying specific increments between price levels.
	@param $Trade ='Spot' //Futures
	$data = '{
		  "symbol": "BTC-USDT",
		  "depth":30,
		  "exchanges": [
			"Binance","CryptoCom","Kraken","KuCoin","Bybit","Okx","GateIO","Mexc","Biconomy","BinanceUS","Bitfinex","Bitget","BitMart","CoinW","Huobi","WhiteBit"
		  ]
		}'; 
	*/ 
	public function SteppedOrderBook($Trade, $data) {
        $url = $this->baseURL . '/Crypto/MultiMarket/' . $Trade . '/SteppedOrderBook';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth, 'Content-Type: application/json-patch+json'];
		try {
			$response = $this->makeRequest($url, 'POST', $headers, $data);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
	/*
	https://doc.sbee.io/api/news
	$language='en';
	$pageSize=20;
	$pageNumber=1;
	$data= $Exchange->News($language,  $pageSize, $pageNumber); 
	*/
	public function News($language,  $pageSize, $pageNumber) {
        $url = $this->baseURL . '/Crypto/News/List?language=' . urlencode($language) . '&pageSize=' . $pageSize. '&pageNumber=' . $pageNumber;
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth]; 
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
	/*
	https://doc.sbee.io/api/country
	Country
	The "country" endpoint provides information about a specific country.
	*/
	public function Country() {
        $url = $this->baseURL . '/Crypto/Country/List';
        $headers = ['accept: text/plain', 'Authorization: Bearer ' . $this->auth]; 
		try {
			$response = $this->makeRequest($url, 'GET', $headers);
			return json_decode($response, true);
		} catch (Exception $e) { 
			return ['ERROR' => $e];
		}
    }
	
}//class end


 



?> 
 
 