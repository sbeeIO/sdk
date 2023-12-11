using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;


class SbeeRest
{
    private string baseURL; //'https://api.sbee.io/api'
    private string auth;

    public SbeeRest(string baseURL, string auth)
    {
        this.baseURL = baseURL;
        this.auth = auth;
    }

    /*
        Server connection functionality 
    */
    private async Task<string> MakeRequest(string url, string method, string[] headers, string? data = null)
    {
        if (method != "GET" && method != "POST")
        {
            throw new Exception("Invalid HTTP method.");
        }

        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), url);

            if (method == "POST" && data != null)
            {
                request.Content = new StringContent(data, Encoding.UTF8, "application/json");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    string[] headerParts = header.Split(':');
                    if (headerParts.Length == 2)
                    {
                        client.DefaultRequestHeaders.Add(headerParts[0], headerParts[1]);
                    }
                }
            }

            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP request failed with status code {response.StatusCode}");
            }

            var x = await response.Content.ReadAsStringAsync();

            return x;
        }
    }

    /*
        https://doc.sbee.io/api/get-system-time
        Get System Time
        Exchange server time information
        @params $Exchange='Binance'
    */
    public async Task<dynamic> SystemTime(string exchange)
    {
        string url = $"{baseURL}/Crypto/{exchange}/SystemTime";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_SystemTime = e.Message };
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
    public async Task<dynamic> RecentTrades(string exchange, string trade, string symbol, string depth)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/RecentTrades?symbol={Uri.EscapeDataString(symbol)}&depth={depth}";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_RecentTrades = e.Message };
        }
    }
    /*
        https://doc.sbee.io/api/public-endpoints/spot/currencies
        Currencies
        Gets all tradable pairs and their quantity or price scales.
        @param $Exchange='Binance'
        @param $Trade ='Spot' //Futures
    */
    public async Task<dynamic> Currencies(string exchange, string trade)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/Currencies";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_Currencies = e.Message };
        }
    }

    /*
        https://doc.sbee.io/api/spot/trading-balances
        Reads wallet information. Gets all cash balances.
        Withdraws all coins when symbol information is left blank
        @param $Exchange='Binance'
        @param $Trade ='Spot' //Futures
        @param $symbol='USDT'
        @param $apiKey='Key...'
        @param $apiSecret='Secret...'
        @param $apiPass='Pass..'
    */
    public async Task<dynamic> TradingBalances(string exchange, string trade, string symbol, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/TradingBalances";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            symbol = symbol,
            apiKey = apiKey,
            apiSecret = apiSecret,
            apiPass = apiPass
        };

        try
        {
            string jsonData = System.Text.Json.JsonSerializer.Serialize(data);
            string response = await MakeRequest(url, "POST", headers, jsonData);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_TradingBalances = e.Message };
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
    public async Task<dynamic> OrderHistory(string exchange, string trade, string symbol, string state, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/OrderHistory";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            symbol = symbol,
            state = state,
            apiKey = apiKey,
            apiSecret = apiSecret,
            apiPass = apiPass
        };

        try
        {
            string jsonData = System.Text.Json.JsonSerializer.Serialize(data);
            string response = await MakeRequest(url, "POST", headers, jsonData);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_OrderHistory = e.Message };
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
        @param $endTime='1689970459999'
        @param $limit='10'
    */
    public async Task<dynamic> KLine(string exchange, string trade, string symbol, string interval, string startTime, string endTime, string limit)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/KLine?symbol={Uri.EscapeDataString(symbol)}&interval={Uri.EscapeDataString(interval)}&startTime={startTime}&endTime={endTime}&limit={limit}";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_KLine = e.Message };
        }
    }

    /*
        https://doc.sbee.io/api/spot/kline-formation
        Kline Formation
        It is a technical analysis tool used in cryptocurrency trading.
        @param $Exchange='Binance'
        @param $Trade ='Spot'
        @param $symbol='BTC-USDT'
        @param $interval='1m, 5m, 15m, 30m, 1h, 4h, 1d, 1M'
        @param $startTime='1689170400000'
        @param $startTime='1689970459999'
        @param $endTime='1603152000'
        @param $limit='100'
        formations = '[ 
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
    public async Task<dynamic> KlineFormation(string exchange, string trade, string symbol, string interval, string limit, string formations, string? startTime = null, string? endTime = null)
    {
        if (startTime == null || endTime == null)
        {
            startTime = null;
            endTime = null;
        }

        string data = @$"{{
            ""symbol"": ""{symbol}"",
            ""interval"": ""{interval}"",
            ""limit"": {limit},
            ""startTime"": {startTime},
            ""endTime"": {endTime},
            ""formations"": {formations}
        }}";

        string url = $"{baseURL}/Crypto/{exchange}/{trade}/KlineFormation";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, data);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_KlineFormation = e.Message };
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
    public async Task<dynamic> OrderBook(string exchange, string trade, string symbol, string depth)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/OrderBook?symbol={Uri.EscapeDataString(symbol)}&depth={depth}";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_OrderBook = e.Message };
        }
    }

    /*
        Tickers
        Gets snapshot information about the latest transaction price, best bid/ask and 24h transaction volume.
        @param 'Binance'
        @param $Trade ='Spot' //Futures
        @param 'BTC-USDT'
    */
    public async Task<dynamic> Tickers(string exchange, string trade, string symbol)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/Tickers?symbol={Uri.EscapeDataString(symbol)}";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_Tickers = e.Message };
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
    public async Task<dynamic> PlaceLimitOrder(string exchange, string trade, string symbol, string clientOrderId, string price, string quoteQuantity, string baseQuantity, string leverage, string contract, string side, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceLimitOrder";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            apiKey,
            apiSecret,
            apiPass,
            symbol,
            ClientOrderId = clientOrderId,
            price,
            quoteQuantity,
            baseQuantity,
            leverage,
            contract,
            side
        };

        string dataJson = JsonConvert.SerializeObject(data);

        try
        {
            string response = await MakeRequest(url, "POST", headers, dataJson);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceLimitOrder = e.Message };
        }
    }

    /*
        https://doc.sbee.io/api/spot/market-order
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
    public async Task<dynamic> PlaceMarketOrder(string exchange, string trade, string symbol, string clientOrderId, string price, string quoteQuantity, string baseQuantity, string leverage, string contract, string side, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceMarketOrder";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            apiKey,
            apiSecret,
            apiPass,
            symbol,
            quoteQuantity,
            baseQuantity,
            ClientOrderId = clientOrderId,
            price,
            leverage,
            contract,
            side
        };

        string dataJson = JsonConvert.SerializeObject(data);

        try
        {
            string response = await MakeRequest(url, "POST", headers, dataJson);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceMarketOrder = e.Message };
        }
    }

    /*
        https://doc.sbee.io/api/spot/limit-stop-loss-order
        Enters a buy or sell stop loss limit order
        @param $Exchange='Binance'
        @param $Trade ='Spot'
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
    public async Task<dynamic> PlaceLimitStopLossOrder(string exchange, string trade, string symbol, string quantity, string clientOrderId, string stopPrice, string orderPrice, string price, string side, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceLimitStopLossOrder";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            apiKey,
            apiSecret,
            apiPass,
            symbol,
            quantity,
            ClientOrderId = clientOrderId,
            stopPrice,
            orderPrice,
            price,
            side
        };

        string dataJson = JsonConvert.SerializeObject(data);

        try
        {
            string response = await MakeRequest(url, "POST", headers, dataJson);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceLimitStopLossOrder = e.Message };
        }
    }


    /*
        https://doc.sbee.io/api/spot/limit-take-profit-order
        Limit Take Profit Order A type of limit order that specifies the exact price at which to close out an open position for a profit.
        @param $Exchange='Binance'
        @param $Trade ='Spot'
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
    public async Task<dynamic> PlaceLimitTakeProfitOrder(string exchange, string trade, string symbol, string quantity, string clientOrderId, string stopPrice, string orderPrice, string price, string side, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceLimitTakeProfitOrder";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            apiKey,
            apiSecret,
            apiPass,
            symbol,
            quantity,
            ClientOrderId = clientOrderId,
            stopPrice,
            orderPrice,
            price,
            side
        };

        string dataJson = JsonConvert.SerializeObject(data);

        try
        {
            string response = await MakeRequest(url, "POST", headers, dataJson);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceLimitTakeProfitOrder = e.Message };
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
    public async Task<dynamic> SetLeverage(string exchange, string trade, string symbol, string leverage, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/SetLeverage";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            apiKey,
            apiSecret,
            apiPass,
            symbol,
            leverage
        };

        string dataJson = JsonConvert.SerializeObject(data);

        try
        {
            string response = await MakeRequest(url, "POST", headers, dataJson);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_SetLeverage = e.Message };
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
    public async Task<dynamic> CancelOrder(string exchange, string trade, string symbol, string orderId, string clientOrderId, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/CancelOrder";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        var data = new
        {
            apiKey,
            apiSecret,
            apiPass,
            symbol,
            orderId,
            clientOrderId
        };

        string dataJson = JsonConvert.SerializeObject(data);

        try
        {
            string response = await MakeRequest(url, "POST", headers, dataJson);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_CancelOrder = e.Message };
        }
    }

    /*
    https://doc.sbee.io/api/spot/batch-processes/cancel-batch-orders
    Cancels a buy or sell bulk order entered in the same wallet 
    @param $Exchange='Binance'
    @param $Trade ='Spot' //Futures
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
     */
    public async Task<dynamic> CancelBatchOrders(string exchange, string trade, string orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/CancelBatchOrders";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, orders);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_CancelBatchOrders = e.Message };
        }
    }

    /*
     https://doc.sbee.io/api/spot/batch-processes/cancel-batch-orders-for-people
     Bulk buy or sell order entered from different wallets cancels
     @param $Exchange='Binance'
     @param $Trade ='Spot' //Futures
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
    */
    public async Task<dynamic> CancelBatchOrdersForPeople(string exchange, string trade, string orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/CancelBatchOrdersForPeople";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, orders);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_CancelBatchOrdersForPeople = e.Message };
        }
    }

    /*
    https://doc.sbee.io/api/spot/batch-processes/batch-market-orders
    Batch Market Orders
    Open more than once market transactions from a single account.
    @param $Exchange='Binance'
    @param $Trade ='Spot' //Futures
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
    */
    public async Task<dynamic> PlaceBatchMarketOrders(string exchange, string trade, string orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceBatchMarketOrders";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, orders);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceBatchMarketOrders = e.Message };
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
                    "apiPass" => ""
                },
                {
                    "symbol": "XRP-USDT", 
                    "apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj.......",
                    "apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOH........",
                    "apiPass" => ""
                }
            ]
        );
    */
    public async Task<dynamic> TradingBalancesForPeople(string exchange, string trade, string orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/TradingBalancesForPeople";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, orders);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_TradingBalancesForPeople = e.Message };
        }
    }

    /*
        https://doc.sbee.io/api/spot/trading/cancel-orders-by-symbol
        Cancels the entered buy and sell orders according to the symbol.
        @param $Exchange='Binance'
        @param $Trade ='Spot' //Futures
        @param $symbol='BTC-USDT'
        @param $apiKey='Key...'
        @param $apiSecret='Secret...'
        @param $apiPass='Pass..'
    */
    public async Task<dynamic> CancelOrdersBySymbol(string exchange, string trade, string symbol, string apiKey, string apiSecret, string apiPass)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/CancelOrdersBySymbol";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            var data = new
            {
                symbol,
                apiKey,
                apiSecret,
                apiPass
            };

            string response = await MakeRequest(url, "POST", headers, JsonConvert.SerializeObject(data));
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_CancelOrdersBySymbol = e.Message };
        }
    }

    /*
        https://doc.sbee.io/api/spot/batch-processes/batch-limit-orders
        Enters bulk limit buy and sell orders from the same wallet
        @param $Exchange='Binance'
        @param $Trade ='Spot' //Futures
        @param $orders = json_encode([
        *  "apiKey" => "tjN8nJZjobFrBHiCuZBmRyyTvp8fG02dmoDj0ibhFP7uX80tVWDpkFTPOLAx4DIM",
        *  "apiSecret" => "ueLdQVF928P5The82uF82hd7qSpU5WOaCV91QVzd8MbgQJMcvkXju3Qey5VRwfge",
        *  "apiPass" => "",
        *  "orders" => [
        *      [
        *          "symbol" => "BTC-USDT",
        *          "clientOrderId" => rand(10000,99999),
        *          "price" => 20000,
        *          "quoteQuantity" => 0,
        *          "baseQuantity" => 0.005,
        *          "side" => "BUY"
        *      ],[
        *          "symbol" => "BTC-USDT",
        *          "clientOrderId" => rand(10000,99999),
        *          "price" => 20000,
        *          "quoteQuantity" => 0,
        *          "baseQuantity" => 0.005,
        *          "side" => "BUY"
        *      ],
        *  ]
        * ]);
    */
    public async Task<dynamic> PlaceBatchLimitOrders(string exchange, string trade, dynamic orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceBatchLimitOrders";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, JsonConvert.SerializeObject(orders));
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceBatchLimitOrders = e.Message };
        }
    }


    /*
     https://doc.sbee.io/api/spot/limit-order-for-people
     Enters bulk limit buy and sell orders from different wallets
     @param $Exchange='Binance'
     @param $Trade ='Spot' //Futures
     @param $orders = '[ 
         {
             "apiKey": "vbcvldv8K5oDjVmZCffg3cNjNS4Ue19VTKJu................",
             "apiSecret": "cvbwrFtldx3VSBfl356c8F5f8swAH2QYo................",
             "apiPass": "string",
             "side": "buy",
             "price": 10000,
             "baseQuantity":0.001,
             "quoteQuantity": 0,
             "cliOrId": "UD01",
             "symbol": "BTC-USDT"
         },{
             "apiKey": "ascvaxvv8K5oDjVmZCffg3cNjNS4Ue19VTKJ.................",
             "apiSecret": "asdasXFtldx3VSBfl356c8F5f8swAH2QY................",
             "apiPass": "string",
             "side": "buy",
             "price": 10000,
             "baseQuantity":0.001,
             "quoteQuantity": 0,
             "cliOrId": "UD02",
             "symbol": "BTC-USDT"
         }
     ]';
    */
    public async Task<dynamic> PlaceLimitOrderForPeople(string exchange, string trade, string orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceLimitOrderForPeople";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, orders);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceLimitOrderForPeople = e.Message };
        }
    }

    /*
      https://doc.sbee.io/api/spot/market-order-for-people
      Enters bulk market buy and sell orders from different wallets
      @param $Exchange='Binance'
      @param $Trade ='Spot' //Futures
      @param $orders ='[
           {
             "symbol": "BTC-USDT",
             "quoteQuantity": 11,
             "baseQuantity": 0,
             "ClientOrderId": "UD01",
             "side": "BUY",
             "apiKey": "ascvaxvv8K5oDjVmZCffg3cNjNS4Ue19VTKJ.................",
             "apiSecret": "asdasXFtldx3VSBfl356c8F5f8swAH2QY................",
             "apiPass": "string"
           }, { 
             "symbol": "BTC-USDT",
             "quoteQuantity": 11,
             "baseQuantity": 0,
             "ClientOrderId": "UD01",
             "side": "BUY",
             "apiKey": "vbcvldv8K5oDjVmZCffg3cNjNS4Ue19VTKJu................",
             "apiSecret": "cvbwrFtldx3VSBfl356c8F5f8swAH2QYo................",
             "apiPass": "string"
           }
         ]';
     */
    public async Task<dynamic> PlaceMarketOrderForPeople(string exchange, string trade, string orders)
    {
        string url = $"{baseURL}/Crypto/{exchange}/{trade}/PlaceMarketOrderForPeople";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };
        string data = orders;

        try
        {
            string response = await MakeRequest(url, "POST", headers, data);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_PlaceMarketOrderForPeople = e.Message };
        }
    }

    /*
     https://doc.sbee.io/api/info-markets
     It provides information about the owned stock exchange and the service endpoints used in the exchange.
    */
    public async Task<dynamic> Markets()
    {
        string url = $"{baseURL}/Crypto/Info/Markets";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_Markets = e.Message };
        }
    }


    /*
 https://doc.sbee.io/api/fintech
 It adjusts the value of currencies relative to each other.
*/
    public async Task<dynamic> MoneyPairValues()
    {
        string url = $"{baseURL}/Fintech/MoneyPairValues";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_MoneyPairValues = e.Message };
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
    public async Task<dynamic> MultiOrderBook(string trade, string data)
    {
        string url = $"{baseURL}/Crypto/MultiMarket/{trade}/OrderBook";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, data);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_MultiOrderBook = e.Message };
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
    public async Task<dynamic> MultiRecentTrades(string trade, string data)
    {
        string url = $"{baseURL}/Crypto/MultiMarket/{trade}/RecentTrades";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string response = await MakeRequest(url, "POST", headers, data);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_MultiRecentTrades = e.Message };
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
    public async Task<dynamic> SteppedOrderBook(string trade, object data)
    {
        string url = $"{baseURL}/Crypto/MultiMarket/{trade}/SteppedOrderBook";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}", "contentType: application/json" };

        try
        {
            string jsonData = JsonConvert.SerializeObject(data);
            string response = await MakeRequest(url, "POST", headers, jsonData);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_SteppedOrderBook = e.Message };
        }
    }

    /*
      https://doc.sbee.io/api/news
      $language='en';
      $pageSize='20';
      $pageNumber='1';
     */
    public async Task<dynamic> News(string language, string pageSize, string pageNumber)
    {
        string url = $"{baseURL}/Crypto/News/List?language={Uri.EscapeDataString(language)}&pageSize={pageSize}&pageNumber={pageNumber}";
        string[] headers = { "accept: text/plain", $"Authorization: Bearer {auth}" };

        try
        {
            string response = await MakeRequest(url, "GET", headers);
            return JsonConvert.DeserializeObject(response);
        }
        catch (Exception e)
        {
            return new { ERROR_News = e.Message };
        }
    }
    static async Task Main()
    {
        // Example of how to use the SbeeRest class
        SbeeRest sbeeRest = new SbeeRest("https://api.sbee.io/api", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIxNDUiLCJuYmYiOjE3MDEwNzA4MzYsImV4cCI6MTk2Mzk4NjAzNn0.DjY98RjYECQBwpPIrXg-v-IvuDviJLCL93XaBDERnYk");

        string[] resultTexts = new string[30];

        string apiKey = "37t6ikIqih2WgNPx2846pggPvDtD9lrNd2jmbtevu6nYzHJUkqMG2jMe7NVvh87w";
        string apiSecret = "on1bbEzKRGzAHo7tVuzaY6eOFVzce9BQz5oZvHVlWJWQa4ckeqZk1zKIUASgI9Ud";
        string apiPass = "";

        //****************************************************************************************************
        //OK
        dynamic result = await sbeeRest.News("en", "20", "1");
        string resultText1 = result.ToString();
        resultTexts[0] = resultText1;

        //****************************************************************************************************
        //OK
        string steppedOrderBookTradeTypeParam = "Spot";
        var steppedOrderBookDataParam = new
        {
            symbol = "BTC-USDT",
            depth = 30,
            exchanges = new string[] {
                    "Binance","CryptoCom","Kraken","KuCoin","Bybit","Okx","GateIO","Mexc","Biconomy","BinanceUS","Bitfinex","Bitget","BitMart","CoinW","Huobi","WhiteBit"
                }
        };
        dynamic result1 = await sbeeRest.SteppedOrderBook(steppedOrderBookTradeTypeParam, steppedOrderBookDataParam);
        string resultText2 = result1.ToString();
        resultTexts[1] = resultText2;

        //****************************************************************************************************
        //OK
        string multiRecentTradesTradeTypeParam = "Spot";
        string multiRecentTradesDataParam = @"{
           ""symbol"": ""BTC-USDT"",
           ""depth"": 50,
           ""exchanges"": [
             ""Binance"",""Binance"",""Kraken"",""KuCoin"",""Bybit"",""OKX"",""GateIO"",""Mexc""
           ]
         }";
        dynamic result2 = await sbeeRest.MultiRecentTrades(multiRecentTradesTradeTypeParam, multiRecentTradesDataParam);
        string resultText3 = result2.ToString();
        resultTexts[2] = resultText3;

        //****************************************************************************************************
        //OK
        string multiOrderBookTradeTypeParam = "Spot";
        string multiOrderBookDataParam = @"{
            ""symbol"": ""ADA-USDT"",
            ""depth"": 50,
            ""precision"": 3,
            ""exchanges"": [
                ""Binance"",""Binance"",""Kraken"",""KuCoin"",""Bybit"",""OKX"",""GateIO"",""Mexc""
            ]
            }";
        dynamic result3 = await sbeeRest.MultiOrderBook(multiOrderBookTradeTypeParam, multiOrderBookDataParam);
        string resultText4 = result3.ToString();
        resultTexts[3] = resultText4;

        //****************************************************************************************************
        //OK
        dynamic result4 = await sbeeRest.MoneyPairValues();
        string resultText5 = result4.ToString();
        resultTexts[4] = resultText5;

        //****************************************************************************************************
        //OK
        dynamic result5 = await sbeeRest.Markets();
        string resultText6 = result5.ToString();
        resultTexts[5] = resultText6;

        //****************************************************************************************************
        //NOT IMPLEMENTED
        string placeMarketOrderForPeopleExchangeParam = "Binance";
        string placeMarketOrderForPeopleTradeTypeParam = "Spot";
        string placeMarketOrderForPeopleDataParam = @"{
            ""symbol"": ""BTC-USDT"",
            ""quoteQuantity"": 36000,
            ""baseQuantity"": 1,
            ""ClientOrderId"": ""1"",
            ""side"": ""BUY"",
            ""apiKey"": ""{apiKey}"",
            ""apiSecret"": ""{apiSecret}"",
            ""apiPass"": ""{apiPass}""
            }";
        dynamic result6 = await sbeeRest.PlaceMarketOrderForPeople(placeMarketOrderForPeopleExchangeParam, placeMarketOrderForPeopleTradeTypeParam, placeMarketOrderForPeopleDataParam);
        string resultText7 = result6.ToString();
        resultTexts[6] = resultText7;

        //****************************************************************************************************
        //NOT IMPLEMENTED
        string placeLimitOrderForPeopleExchangeParam = "Binance";
        string placeLimitOrderForPeopleTradeTypeParam = "Spot";
        string placeLimitOrderForPeopleDataParam = @"[
            {
                ""apiKey"": ""{apiKey}"",
                ""apiSecret"": ""{apiSecret}"",
                ""apiPass"": ""{apiPass}""
                ""side"": ""SELL"",
                ""price"": 36000,
                ""baseQuantity"": 1,
                ""quoteQuantity"": 36000,
                ""cliOrId"": ""1"",
                ""symbol"": ""BTC-USDT""
            },{
                ""apiKey"": ""{apiKey}"",
                ""apiSecret"": ""{apiSecret}"",
                ""apiPass"": ""{apiPass}""
                ""side"": ""SELL"",
                ""price"": 36000,
                ""baseQuantity"": 1,
                ""quoteQuantity"": 36000,
                ""cliOrId"": ""1"",
                ""symbol"": ""BTC-USDT""
            }
            ]";
        dynamic result7 = await sbeeRest.PlaceLimitOrderForPeople(placeLimitOrderForPeopleExchangeParam, placeLimitOrderForPeopleTradeTypeParam, placeLimitOrderForPeopleDataParam);
        string resultText8 = "";
        if (result7 != null)
        {
            resultText8 = result7.ToString();
        }
        resultTexts[7] = resultText8;

        //****************************************************************************************************
        //NOT IMPLEMENTED
        string placeBatchLimitOrdersExchangeParam = "Binance";
        string placeBatchLimitOrdersTradeTypeParam = "Spot";
        string placeBatchLimitOrdersDataParam = @"{
            ""apiKey"": ""{apiKey}"",
            ""apiSecret"": ""{apiSecret}"",
            ""apiPass"": ""{apiPass}""
            ""orders"": [
                {
                    ""symbol"": ""BTC-USDT"",
                    ""clientOrderId"": ""1"",
                    ""price"": 36000,
                    ""quoteQuantity"": 36000,
                    ""baseQuantity"": 1,
                    ""side"": ""BUY""
                },{
                    ""symbol"": ""BTC-USDT"",
                    ""clientOrderId"": ""1"",
                    ""price"": 36000,
                    ""quoteQuantity"": 36000,
                    ""baseQuantity"": 1,
                    ""side"": ""BUY""
                },
            ]
            }";
        dynamic result8 = await sbeeRest.PlaceBatchLimitOrders(placeBatchLimitOrdersExchangeParam, placeBatchLimitOrdersTradeTypeParam, placeBatchLimitOrdersDataParam);
        string resultText9 = result8.ToString();
        resultTexts[8] = resultText9;

        //****************************************************************************************************
        //OK
        string cancelOrdersBySymbolExchangeParam = "Binance";
        string cancelOrdersBySymbolTradeTypeParam = "Spot";
        string cancelOrdersBySymbolSymbolParam = "BTC-USDT";
        string cancelOrdersBySymbolApiKeyParam = "{apiKey}";
        string cancelOrdersBySymbolApiSecretParam = "{apiSecret}";
        string cancelOrdersBySymbolApiPassParam = "{apiPass}";
        dynamic result9 = await sbeeRest.CancelOrdersBySymbol(cancelOrdersBySymbolExchangeParam, cancelOrdersBySymbolTradeTypeParam, cancelOrdersBySymbolSymbolParam, cancelOrdersBySymbolApiKeyParam, cancelOrdersBySymbolApiSecretParam, cancelOrdersBySymbolApiPassParam);
        string resultText10 = result9.ToString();
        resultTexts[9] = resultText10;

        //****************************************************************************************************
        //OK
        string tradingBalancesForPeopleExchangeParam = "Binance";
        string tradingBalancesForPeopleTradeTypeParam = "Spot";
        string tradingBalancesForPeopleDataParam = @"[
            {
                ""symbol"": ""BTC"", 
                ""apiKey"": ""{apiKey}"",
                ""apiSecret"": ""{apiSecret}"",
                ""apiPass"": ""{apiPass}""
            }
        ]";
        dynamic result10 = await sbeeRest.TradingBalancesForPeople(tradingBalancesForPeopleExchangeParam, tradingBalancesForPeopleTradeTypeParam, tradingBalancesForPeopleDataParam);
        string resultText11 = result10.ToString();
        resultTexts[10] = resultText11;

        //****************************************************************************************************
        //OK
        string cancelBatchOrdersForPeopleExchangeParam = "Binance";
        string cancelBatchOrdersForPeopleTradeTypeParam = "Spot";
        string cancelBatchOrdersForPeopleDataParam = @"[
            {
                ""symbol"": ""BTC-USDT"",
                ""orderId"": ""1"",
                ""clientOrderId"": ""1"",
                ""apiKey"": ""{apiKey}"",
                ""apiSecret"": ""{apiSecret}"",
                ""apiPass"": ""{apiPass}""
            }
            ]";
        dynamic result11 = await sbeeRest.CancelBatchOrdersForPeople(cancelBatchOrdersForPeopleExchangeParam, cancelBatchOrdersForPeopleTradeTypeParam, cancelBatchOrdersForPeopleDataParam);
        string resultText12 = result11.ToString();
        resultTexts[11] = resultText12;

        //****************************************************************************************************
        //NOT IMPLEMENTED
        string cancelBatchOrdersExchangeParam = "Binance";
        string cancelBatchOrdersTradeTypeParam = "Spot";
        string cancelBatchOrdersDataParam = @"{
            ""apiKey"": ""{apiKey}"",
            ""apiSecret"": ""{apiSecret}"",
            ""apiPass"": ""{apiPass}""
            ""orders"": [
                [
                    ""symbol"": ""BTC-USDT"",
                    ""clientOrderId"": ""1"",
                    ""orderId"": ""1"", 
                ],[
                    ""symbol"": ""BTC-USDT"",
                    ""clientOrderId"": ""2"",
                    ""orderId"": ""2"", 
                ],
            ]
            }";
        dynamic result12 = await sbeeRest.CancelBatchOrders(cancelBatchOrdersExchangeParam, cancelBatchOrdersTradeTypeParam, cancelBatchOrdersDataParam);
        string resultText13 = "";
        if (result12 != null)
        {
            resultText13 = result12.ToString();
            resultTexts[12] = resultText13;
        }

        resultTexts[12] = resultText13;

        //****************************************************************************************************
        //OK
        string cancelOrderExchangeParam = "Binance";
        string cancelOrderTradeParam = "Spot";
        string cancelOrderSymbolParam = "BTC-USDT";
        string cancelOrderOrderIdParam = "123";
        string cancelOrderClientOrderIdParam = "1";
        string cancelOrderApiKeyParam = "{apiKey}";
        string cancelOrderApiSecretParam = "{apiSecret}";
        string cancelOrderApiPassParam = "{apiPass}";
        dynamic result14 = await sbeeRest.CancelOrder(cancelOrderExchangeParam, cancelOrderTradeParam, cancelOrderSymbolParam, cancelOrderOrderIdParam, cancelOrderClientOrderIdParam, cancelOrderApiKeyParam, cancelOrderApiSecretParam, cancelOrderApiPassParam);
        string resultText15 = result14.ToString();
        resultTexts[14] = resultText15;

        //****************************************************************************************************
        //OK
        string setLeverageExchangeParam = "Binance";
        string setLeverageTradeParam = "Futures";
        string setLeverageSymbolParam = "BTC-USDT";
        string setLeverageLeverageParam = "5";
        string setLeverageApiKeyParam = "{apiKey}";
        string setLeverageApiSecretParam = "{apiSecret}";
        string setLeverageApiPassParam = "{apiPass}";
        dynamic result15 = await sbeeRest.SetLeverage(setLeverageExchangeParam, setLeverageTradeParam, setLeverageSymbolParam, setLeverageLeverageParam, setLeverageApiKeyParam, setLeverageApiSecretParam, setLeverageApiPassParam);
        string resultText16 = result15.ToString();
        resultTexts[15] = resultText16;

        //****************************************************************************************************
        //OK
        string placeLimitTakeProfitOrderExchangeParam = "Binance";
        string placeLimitTakeProfitOrderTradeParam = "Spot";
        string placeLimitTakeProfitOrderSymbolParam = "BTC-USDT";
        string placeLimitTakeProfitOrderQuantityParam = "1";
        string placeLimitTakeProfitOrderClientOrderIdParam = "1";
        string placeLimitTakeProfitOrderStopPriceParam = "36010";
        string placeLimitTakeProfitOrderOrderPriceParam = "35950";
        string placeLimitTakeProfitOrderPriceParam = "36000";
        string placeLimitTakeProfitOrderSideParam = "SELL";
        string placeLimitTakeProfitOrderApiKeyParam = "{apiKey}";
        string placeLimitTakeProfitOrderApiSecretParam = "{apiSecret}";
        string placeLimitTakeProfitOrderApiPassParam = "{apiPass}";
        dynamic result16 = await sbeeRest.PlaceLimitTakeProfitOrder(
            placeLimitTakeProfitOrderExchangeParam,
            placeLimitTakeProfitOrderTradeParam,
             placeLimitTakeProfitOrderSymbolParam,
              placeLimitTakeProfitOrderQuantityParam,
               placeLimitTakeProfitOrderClientOrderIdParam,
                placeLimitTakeProfitOrderStopPriceParam,
                 placeLimitTakeProfitOrderOrderPriceParam,
                  placeLimitTakeProfitOrderPriceParam,
                    placeLimitTakeProfitOrderSideParam,
                     placeLimitTakeProfitOrderApiKeyParam,
                      placeLimitTakeProfitOrderApiSecretParam,
                       placeLimitTakeProfitOrderApiPassParam);
        string resultText17 = result16.ToString();
        resultTexts[16] = resultText17;

        //****************************************************************************************************
        //OK
        string placeLimitStopLossOrderExchangeParam = "Binance";
        string placeLimitStopLossOrderTradeParam = "Spot";
        string placeLimitStopLossOrderSymbolParam = "BTC-USDT";
        string placeLimitStopLossOrderQuantityParam = "1";
        string placeLimitStopLossOrderClientOrderIdParam = "1";
        string placeLimitStopLossOrderStopPriceParam = "36010";
        string placeLimitStopLossOrderOrderPriceParam = "35950";
        string placeLimitStopLossOrderPriceParam = "35950";
        string placeLimitStopLossOrderSideParam = "SELL";
        string placeLimitStopLossOrderApiKeyParam = "{apiKey}";
        string placeLimitStopLossOrderApiSecretParam = "{apiSecret}";
        string placeLimitStopLossOrderApiPassParam = "{apiPass}";

        dynamic result17 = await sbeeRest.PlaceLimitStopLossOrder(
            placeLimitStopLossOrderExchangeParam,
            placeLimitStopLossOrderTradeParam,
             placeLimitStopLossOrderSymbolParam,
              placeLimitStopLossOrderQuantityParam,
               placeLimitStopLossOrderClientOrderIdParam,
                placeLimitStopLossOrderStopPriceParam,
                 placeLimitStopLossOrderOrderPriceParam,
                  placeLimitStopLossOrderPriceParam,
                    placeLimitStopLossOrderSideParam,
                     placeLimitStopLossOrderApiKeyParam,
                      placeLimitStopLossOrderApiSecretParam,
                       placeLimitStopLossOrderApiPassParam);
        string resultText18 = result17.ToString();
        resultTexts[17] = resultText18;

        //****************************************************************************************************
        //OK
        string placeMarketOrderExchangeParam = "Binance";
        string placeMarketOrderTradeParam = "Futures";
        string placeMarketOrderSymbolParam = "BTC-USDT";
        string placeMarketOrderClientOrderIdParam = "1";
        string placeMarketOrderPriceParam = "36000";
        string placeMarketOrderQuoteQuantityParam = "36000";
        string placeMarketOrderBaseQuantityParam = "1";
        string placeMarketOrderLeverageParam = "5";
        string placeMarketOrderContractParam = "1";
        string placeMarketOrderSideParam = "Buy";
        string placeMarketOrderApiKeyParam = "{apiKey}";
        string placeMarketOrderApiSecretParam = "{apiSecret}";
        string placeMarketOrderApiPassParam = "{apiPass}";
        dynamic result18 = await sbeeRest.PlaceMarketOrder(
            placeMarketOrderExchangeParam,
            placeMarketOrderTradeParam,
             placeMarketOrderSymbolParam,
              placeMarketOrderClientOrderIdParam,
               placeMarketOrderPriceParam,
                placeMarketOrderQuoteQuantityParam,
                 placeMarketOrderBaseQuantityParam,
                 placeMarketOrderLeverageParam,
                 placeMarketOrderContractParam,
                  placeMarketOrderSideParam,
                   placeMarketOrderApiKeyParam,
                    placeMarketOrderApiSecretParam,
                     placeMarketOrderApiPassParam);
        string resultText19 = result18.ToString();
        resultTexts[18] = resultText19;

        //****************************************************************************************************
        //OK
        string placeLimitOrderApiKeyParam = "{apiKey}";
        string placeLimitOrderApiSecretParam = "{apiSecret}";
        string placeLimitOrderApiPassParam = "{apiPass}";
        string placeLimitOrderSymbolParam = "BTC-USDT";
        string placeLimitOrderClientOrderIdParam = "1";
        string placeLimitOrderPriceParam = "36000";
        string placeLimitOrderQuoteQuantityParam = "36000";
        string placeLimitOrderBaseQuantityParam = "1";
        string placeLimitOrderLeverageParam = "SELL";
        string placeLimitOrderContractParam = "SELL";
        string placeLimitOrderSideParam = "SELL";
        string placeLimitOrderExchangeParam = "Binance";
        string placeLimitOrderTradeParam = "Spot";
        dynamic result19 = await sbeeRest.PlaceLimitOrder(
            placeLimitOrderExchangeParam,
            placeLimitOrderTradeParam,
             placeLimitOrderSymbolParam,
              placeLimitOrderClientOrderIdParam,
               placeLimitOrderPriceParam,
                placeLimitOrderQuoteQuantityParam,
                 placeLimitOrderBaseQuantityParam,
                  placeLimitOrderLeverageParam,
                   placeLimitOrderContractParam,
                    placeLimitOrderSideParam,
                     placeLimitOrderApiKeyParam,
                      placeLimitOrderApiSecretParam,
                       placeLimitOrderApiPassParam);
        string resultText20 = result19.ToString();
        resultTexts[19] = resultText20;

        //****************************************************************************************************
        //OK
        string tickersExchangeParam = "Binance";
        string tickersTradeParam = "Spot";
        string tickersSymbolParam = "BTC-USDT";
        dynamic result20 = await sbeeRest.Tickers(
            tickersExchangeParam,
            tickersTradeParam,
             tickersSymbolParam);
        string resultText21 = result20.ToString();
        resultTexts[20] = resultText21;

        //****************************************************************************************************
        //OK
        string orderBookExchangeParam = "Binance";
        string orderBookTradeParam = "Spot";
        string orderBookSymbolParam = "BTC-USDT";
        string orderBookDepthParam = "10";
        dynamic result21 = await sbeeRest.OrderBook(
            orderBookExchangeParam,
            orderBookTradeParam,
             orderBookSymbolParam,
              orderBookDepthParam);
        string resultText22 = result21.ToString();
        resultTexts[21] = resultText22;

        //****************************************************************************************************
        // ???
        string klineFormationExchangeParam = "Binance";
        string klineFormationTradeParam = "Spot";
        string klineFormationSymbolParam = "BTC-USDT";
        string klineFormationIntervalParam = "1m";
        string klineFormationLimitParam = "100";
        string klineFormationFormationsParam = @"[ 
                          {
                ""Formation"": ""MAX"",
                              ""TimePeriod"": 30,
                              ""Source"": ""close""
                          }, 
                          {
                ""Formation"": ""DX"",
                              ""TimePeriod"": 14
                          },
                          {
                ""Formation"": ""MACD"",
                              ""FastPeriod"": 12,
                              ""SlowPeriod"": 26,
                              ""SignalPeriod"": 9,
                              ""Source"": ""close""
                          } 
                      ]";
        dynamic result22 = await sbeeRest.KlineFormation(
            klineFormationExchangeParam,
            klineFormationTradeParam,
             klineFormationSymbolParam,
              klineFormationIntervalParam,
                 klineFormationLimitParam,
                  klineFormationFormationsParam);
        string resultText23 = result22.ToString();
        resultTexts[22] = resultText23;

        //****************************************************************************************************
        // ???
        string klineExchangeParam = "Binance";
        string klineTradeParam = "Spot";
        string klineSymbolParam = "BTC-USDT";
        string klineIntervalParam = "1m";
        string klineStartTimeParam = "1689170400000";
        string klineEndTimeParam = "1689970459999";
        string klineLimitParam = "10";
        dynamic result23 = await sbeeRest.KLine(
            klineExchangeParam,
            klineTradeParam,
             klineSymbolParam,
              klineIntervalParam,
              klineStartTimeParam,
                klineEndTimeParam,
                 klineLimitParam);
        string resultText24 = result23.ToString();
        resultTexts[23] = resultText24;

        //****************************************************************************************************
        //OK
        string orderHistoryExchangeParam = "Binance";
        string orderHistoryTradeParam = "Spot";
        string orderHistorySymbolParam = "BTC-USDT";
        string orderHistoryStateParam = "NEW,ALL,FILLED,CANCELED";
        string orderHistoryApiKeyParam = "{apiKey}";
        string orderHistoryApiSecretParam = "{apiSecret}";
        string orderHistoryApiPassParam = "{apiPass}";
        dynamic result24 = await sbeeRest.OrderHistory(
            orderHistoryExchangeParam,
            orderHistoryTradeParam,
             orderHistorySymbolParam,
              orderHistoryStateParam,
               orderHistoryApiKeyParam,
                orderHistoryApiSecretParam,
                 orderHistoryApiPassParam);
        string resultText25 = result24.ToString();
        resultTexts[24] = resultText25;

        //****************************************************************************************************
        //OK
        string tradingBalanceExchangeParam = "Binance";
        string tradingBalanceTradeParam = "Spot";
        string tradingBalanceSymbolParam = "USDT";
        string tradingBalanceApiKeyParam = $"{apiKey}";
        string tradingBalanceApiSecretParam = $"{apiSecret}";
        string tradingBalanceApiPassParam = $"{apiPass}";
        dynamic result25 = await sbeeRest.TradingBalances(
            tradingBalanceExchangeParam,
            tradingBalanceTradeParam,
             tradingBalanceSymbolParam,
              tradingBalanceApiKeyParam,
               tradingBalanceApiSecretParam,
                tradingBalanceApiPassParam);
        string resultText26 = result25.ToString();
        resultTexts[25] = resultText26;

        //****************************************************************************************************
        //OK
        string currenciesExchangeParam = "Binance";
        string currenciesTradeParam = "Spot";
        dynamic result26 = await sbeeRest.Currencies(
            currenciesExchangeParam,
            currenciesTradeParam);
        string resultText27 = result26.ToString();
        resultTexts[26] = resultText27;

        //****************************************************************************************************
        //OK
        string recentTradesExchangeParam = "Binance";
        string recentTradesTradeParam = "Spot";
        string recentTradesSymbolParam = "BTC-USDT";
        string recentTradesDepthParam = "10";
        dynamic result27 = await sbeeRest.RecentTrades(
            recentTradesExchangeParam,
            recentTradesTradeParam,
             recentTradesSymbolParam,
              recentTradesDepthParam);
        string resultText28 = result27.ToString();
        resultTexts[27] = resultText28;

        //****************************************************************************************************
        //OK
        string systemTimeExchangeParam = "Binance";
        dynamic result28 = await sbeeRest.SystemTime(
            systemTimeExchangeParam);
        string resultText29 = result28.ToString();
        resultTexts[28] = resultText29;

        //****************************************************************************************************
        //
        string placeBatchMarketOrdersExchangeParam = "Binance";
        string placeBatchMarketOrdersTradeTypeParam = "Spot";
        string placeBatchMarketOrdersDataParam = @"{
            ""apiKey"": ""{apiKey}"",
            ""apiSecret"": ""{apiSecret}"",
            ""apiPass"": ""{apiPass}""
            ""orders"": [
                {
                    ""symbol"": ""BTC-USDT"",
                    ""quoteQuantity"": 0,
                    ""baseQuantity"": 0.005,
                    ""clientOrderId"": ""1"",
                    ""side"": ""SELL""
                }
            ]
            }";
        dynamic result29 = await sbeeRest.PlaceBatchMarketOrders(placeBatchMarketOrdersExchangeParam, placeBatchMarketOrdersTradeTypeParam, placeBatchMarketOrdersDataParam);
        string resultText30 = "";
        if (result29 != null)
        {
            resultText30 = result29.ToString();
        }
        resultTexts[29] = resultText30;

        //****************************************************************************************************

        string filePath = "result1.txt";

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            foreach (string resultText in resultTexts)
            {

                writer.WriteLine(resultText);
                writer.WriteLine(Environment.NewLine);
                writer.WriteLine("\n****************************************************************************************************\n");
                writer.WriteLine(Environment.NewLine);
            }
        }


    }
}
