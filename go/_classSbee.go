/*
WHAT IS SBEE?
SBEE EXCHANGE ENGINE (SBEE) IS AN INTERMEDIARY SOFTWARE THAT ACTS AS A LINK CONNECTING CRYPTOCURRENCY EXCHANGES AND APPLICATIONS.
BY INTEGRATING WITH SBEE API SERVICES, APPLICATIONS CAN ESTABLISH CONNECTIONS TO MULTIPLE CRYPTOCURRENCY EXCHANGES SEAMLESSLY.
THIS INTEGRATION PROVIDES USERS WITH THE FLEXIBILITY TO TRADE ON VARIOUS EXCHANGES, ACCESS LIVE DATA, AND PERFORM DIVERSE TRANSACTIONS ALL FROM A CENTRALIZED PLATFORM.
TO UTILIZE THIS LIBRARY, YOU NEED TO GENERATE A FREE BEARER TOKEN FROM WWW.SBEE.IO.
BY ENTERING THE NAME OF THE EXCHANGE YOU WANT TO OPERATE ON AND PROVIDING YOUR TRANSACTION DETAILS,
YOU CAN EASILY AND QUICKLY EXECUTE YOUR TRANSACTIONS. FOR DETAILED INFORMATION ABOUT THE SERVICE, YOU CAN ACCESS DOC.SBEE.IO.
*/
package main

import (
	"encoding/json"
	"errors"
	"fmt"
	ioutil "io/ioutil"
	"net/http"
	"net/url"
)

type SbeeRest struct {
	baseURL string
	auth    string
}

func (s *SbeeRest) makeRequest(url, method string, headers map[string]string, data string) ([]byte, error) {
	if method != "GET" && method != "POST" {
		return nil, errors.New("invalid HTTP method")
	}

	client := &http.Client{}
	req, err := http.NewRequest(method, url, nil)
	if err != nil {
		return nil, err
	}

	for key, value := range headers {
		req.Header.Set(key, value)
	}

	resp, err := client.Do(req)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}

	return body, nil
}

/*
https://doc.sbee.io/api/get-system-time
Get System Time
Exchange server time information
@params Exchange='Binance'
*/
func (s *SbeeRest) SystemTime(Exchange string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/SystemTime", s.baseURL, Exchange)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, fmt.Errorf("SystemTime request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("SystemTime unmarshal error: %v", err)
	}

	return result, nil
}

/*
https://doc.sbee.io/api/public-endpoints/spot/recent-trades
Recent Trades
The purpose of using the "Recent Trades" method in cryptocurrency futures trading is to view the recent trades that have taken place on a specific futures contract.
Past fulfilled buy and sell orders
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params limit='20'
*/
func (s *SbeeRest) RecentTrades(Exchange, Trade, symbol, depth string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/RecentTrades?symbol=%s&depth=%s", s.baseURL, Exchange, Trade, symbol, depth)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
	https://doc.sbee.io/api/public-endpoints/spot/currencies
	Currencies
	Gets all tradable pairs and their quantity or price scales.
	@params Exchange='Binance'
	@params Trade ='Spot' //Futures
*/

func (s *SbeeRest) Currencies(Exchange, Trade string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/Currencies", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/trading-balances
Reads wallet information.Gets all cash balances.
Withdraws all coins when symbol information is left blank
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='USDT'
@params apiKey='Key...'
@params apiSecret='Secret...'
@params apiPass='Pass..'
*/
func (s *SbeeRest) TradingBalances(Exchange, Trade, symbol, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/TradingBalances", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	data := fmt.Sprintf(`{
		"symbol": "%s",
		"apiKey": "%s",
		"apiSecret": "%s",
		"apiPass": "%s"
	}`, symbol, apiKey, apiSecret, apiPass)

	response, err := s.makeRequest(url, "POST", headers, data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
	https://doc.sbee.io/api/spot/order-history
	My buy and sell orders
	@params Exchange='Binance'
	@params Trade ='Spot' //Futures
	@params symbol='BTC-USDT'
	@params state='NEW,ALL,FILLED,CANCELED'
	@params apiKey='Key...'
	@params apiSecret='Secret...'
	@params apiPass='Pass..'
*/

func (s *SbeeRest) OrderHistory(Exchange, Trade, symbol, state, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/OrderHistory", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	data := fmt.Sprintf(`{
		"symbol": "%s",
		"state": "%s",
		"apiKey": "%s",
		"apiSecret": "%s",
		"apiPass": "%s"
	}`, symbol, state, apiKey, apiSecret, apiPass)

	response, err := s.makeRequest(url, "POST", headers, data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/public-endpoints/spot/kline
KLine
Kline/candlestick bars for a symbol. The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
Pulls historical candle information
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params interval='1m, 5m, 15m, 30m, 1h, 4h, 1d, 1M'
@params startTime='1689170400000'
@params startTime='1689970459999'
@params endTime='1603152000'
@params limit='10'
*/
func (s *SbeeRest) KLine(Exchange, Trade, symbol, interval, startTime, endTime string, limit int) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/KLine?symbol=%s&interval=%s&startTime=%s&endTime=%s&limit=%d", s.baseURL, Exchange, Trade, symbol, interval, startTime, endTime, limit)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/kline-formation
Kline Formation
It is a technical analysis tool used in cryptocurrency trading.
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params interval='1m, 5m, 15m, 30m, 1h, 4h, 1d, 1M'
@params startTime='1689170400000'
@params startTime='1689970459999'
@params endTime='1603152000'
@params limit='100'
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
func (s *SbeeRest) KlineFormation(Exchange, Trade, symbol, interval string, limit, formations int, startTime, endTime interface{}) (map[string]interface{}, map[string]interface{}) {
	if startTime == nil || endTime == nil {
		startTime = nil
		endTime = nil
	}
	data := fmt.Sprintf(`{
		"symbol": "%s",
		"interval": "%s",
		"limit": %d,
		"startTime": %v,
		"endTime": %v,
		"formations": %d
	}`, symbol, interval, limit, startTime, endTime, formations)

	url := fmt.Sprintf("%s/Crypto/%s/%s/KlineFormation", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
Order Book
Gets an instant list of all open orders for a product.
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params depth='20'
*/
func (s *SbeeRest) OrderBook(Exchange, Trade, symbol string, depth int) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/OrderBook?symbol=%s&depth=%d", s.baseURL, Exchange, Trade, symbol, depth)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
Tickers
Gets snapshot information about the latest transaction price, best bid/ask and 24h transaction volume.
@params 'Binance'
@params Trade ='Spot' //Futures
@params 'BTC-USDT'
*/
func (s *SbeeRest) Tickers(Exchange, Trade, symbol string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/Tickers?symbol=%s", s.baseURL, Exchange, Trade, symbol)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/limit-order
Enters a buy or sell order
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params ClientOrderId='ID3231'
@params price='16000'
@params quoteQuantity='0'
@params baseQuantity='0.005'
@params side='BUY'
@params apiKey='Key...'
@params apiSecret='Secret...'
@params apiPass='Pass..'
*/
func (s *SbeeRest) PlaceLimitOrder(Exchange, Trade, symbol, ClientOrderId, price, quoteQuantity, baseQuantity, side, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceLimitOrder", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}
	data := map[string]interface{}{
		"apiKey":        apiKey,
		"apiSecret":     apiSecret,
		"apiPass":       apiPass,
		"symbol":        symbol,
		"ClientOrderId": ClientOrderId,
		"price":         price,
		"quoteQuantity": quoteQuantity,
		"baseQuantity":  baseQuantity,
		"side":          side,
	}
	dataJson, err := json.Marshal(data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJson))
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/market-order
Executes a buy or sell order at market price
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT';
@params ClientOrderId='ID326511'
@params price='26000'
@params quoteQuantity='15'
@params baseQuantity='0'
@params side='BUY'
@params apiKey='Key...'
@params apiSecret='Secret...'
@params apiPass='Pass..'
*/
func (s *SbeeRest) PlaceMarketOrder(Exchange, Trade, symbol, ClientOrderId, price, quoteQuantity, baseQuantity string, leverage, contract int, side, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceMarketOrder", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}
	data := map[string]interface{}{
		"apiKey":        apiKey,
		"apiSecret":     apiSecret,
		"apiPass":       apiPass,
		"symbol":        symbol,
		"ClientOrderId": ClientOrderId,
		"price":         price,
		"quoteQuantity": quoteQuantity,
		"baseQuantity":  baseQuantity,
		"leverage":      leverage,
		"contract":      contract,
		"side":          side,
	}
	dataJson, err := json.Marshal(data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJson))
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/limit-stop-loss-order
Enters a buy or sell stop loss limit order
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params quantity='0.0005'
@params ClientOrderId='ID653'
@params stopPrice='28000'
@params orderPrice='0'
@params price='27500'
@params trailingDelta='0'
@params side='BUY'
@params apiKey='Key...'
@params apiSecret='Secret...'
@params apiPass='Pass..'
*/
func (s *SbeeRest) PlaceLimitStopLossOrder(Exchange, Trade, symbol, quantity, ClientOrderId, stopPrice, orderPrice, price, trailingDelta, side, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceLimitStopLossOrder", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}
	data := map[string]interface{}{
		"apiKey":        apiKey,
		"apiSecret":     apiSecret,
		"apiPass":       apiPass,
		"symbol":        symbol,
		"quantity":      quantity,
		"ClientOrderId": ClientOrderId,
		"stopPrice":     stopPrice,
		"orderPrice":    orderPrice,
		"price":         price,
		"trailingDelta": trailingDelta,
		"side":          side,
	}
	dataJson, err := json.Marshal(data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJson))
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/limit-take-profit-order
Limit Take Profit Order A type of limit order that specifies the exact price at which to close out an open position for a profit.
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params quantity='0.005'
@params ClientOrderId='ID653323'
@params stopPrice='25000'
@params orderPrice='22000'
@params price='20000'
@params trailingDelta='0'
@params apiKey='Key...'
@params apiSecret='Secret...'
@params apiPass='Pass..'
*/
func (s *SbeeRest) PlaceLimitTakeProfitOrder(Exchange, Trade, symbol, quantity, ClientOrderId, stopPrice, orderPrice, price, trailingDelta, side, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceLimitTakeProfitOrder", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}
	data := map[string]interface{}{
		"apiKey":        apiKey,
		"apiSecret":     apiSecret,
		"apiPass":       apiPass,
		"symbol":        symbol,
		"quantity":      quantity,
		"ClientOrderId": ClientOrderId,
		"stopPrice":     stopPrice,
		"orderPrice":    orderPrice,
		"price":         price,
		"trailingDelta": trailingDelta,
		"side":          side,
	}
	dataJson, err := json.Marshal(data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJson))
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/private-endpoints/futures/set-leverage
Set Leverage
Allows the leverage value to be defined.
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params leverage='5'
*/
func (s *SbeeRest) SetLeverage(Exchange, Trade, symbol, leverage, apiKey, apiSecret, apiPass string) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/SetLeverage", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}
	data := map[string]interface{}{
		"apiKey":    apiKey,
		"apiSecret": apiSecret,
		"apiPass":   apiPass,
		"symbol":    symbol,
		"leverage":  leverage,
	}
	dataJson, err := json.Marshal(data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJson))
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

/*
https://doc.sbee.io/api/spot/cancel-order
Cancels the entered buy or sell order
@params Exchange='Binance'
@params Trade ='Spot' //Futures
@params symbol='BTC-USDT'
@params orderId='43523123123'
@params clientOrderId='ID3421'
@params apiKey='Key...'
@params apiSecret='Secret...'
@params apiPass='Pass..'
*/
func (s *SbeeRest) CancelOrder(Exchange, Trade, symbol, apiKey, apiSecret, apiPass string, orderId, clientOrderId int) (map[string]interface{}, map[string]interface{}) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/CancelOrder", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}
	data := map[string]interface{}{
		"apiKey":        apiKey,
		"apiSecret":     apiSecret,
		"apiPass":       apiPass,
		"symbol":        symbol,
		"orderId":       orderId,
		"clientOrderId": clientOrderId,
	}
	dataJson, err := json.Marshal(data)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJson))
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
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
func (s *SbeeRest) CancelBatchOrders(Exchange, Trade, orders, apiKey, apiSecret, apiPass string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/CancelBatchOrders", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	data := map[string]interface{}{
		"orders":    orders,
		"apiKey":    apiKey,
		"apiSecret": apiSecret,
		"apiPass":   apiPass,
	}

	dataJSON, err := json.Marshal(data)
	if err != nil {
		return nil, fmt.Errorf("CancelBatchOrders marshal error: %v", err)
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJSON))
	if err != nil {
		return nil, fmt.Errorf("CancelBatchOrders request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("CancelBatchOrders unmarshal error: %v", err)
	}

	return result, nil
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
func (s *SbeeRest) CancelBatchOrdersForPeople(Exchange, Trade, orders string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/CancelBatchOrdersForPeople", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, orders)
	if err != nil {
		return nil, fmt.Errorf("CancelBatchOrdersForPeople request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("CancelBatchOrdersForPeople unmarshal error: %v", err)
	}

	return result, nil
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
func (s *SbeeRest) PlaceBatchMarketOrders(Exchange, Trade, orders string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceBatchMarketOrders", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, orders)
	if err != nil {
		return nil, fmt.Errorf("PlaceBatchMarketOrders request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("PlaceBatchMarketOrders unmarshal error: %v", err)
	}

	return result, nil
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
func (s *SbeeRest) TradingBalancesForPeople(Exchange, Trade, orders string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/TradingBalancesForPeople", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, orders)
	if err != nil {
		return nil, fmt.Errorf("TradingBalancesForPeople request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("TradingBalancesForPeople unmarshal error: %v", err)
	}

	return result, nil
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
func (s *SbeeRest) CancelOrdersBySymbol(Exchange, Trade, symbol, apiKey, apiSecret, apiPass string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/CancelOrdersBySymbol", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	data := map[string]string{
		"symbol":    symbol,
		"apiKey":    apiKey,
		"apiSecret": apiSecret,
		"apiPass":   apiPass,
	}

	dataJSON, err := json.Marshal(data)
	if err != nil {
		return nil, fmt.Errorf("CancelOrdersBySymbol json marshal error: %v", err)
	}

	response, err := s.makeRequest(url, "POST", headers, string(dataJSON))
	if err != nil {
		return nil, fmt.Errorf("CancelOrdersBySymbol request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("CancelOrdersBySymbol unmarshal error: %v", err)
	}

	return result, nil
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
func (s *SbeeRest) PlaceBatchLimitOrders(Exchange, Trade string, orders interface{}) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceBatchLimitOrders", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	// Convert orders to JSON
	ordersJSON, err := json.Marshal(orders)
	if err != nil {
		return nil, fmt.Errorf("PlaceBatchLimitOrders json marshal error: %v", err)
	}

	response, err := s.makeRequest(url, "POST", headers, string(ordersJSON))
	if err != nil {
		return nil, fmt.Errorf("PlaceBatchLimitOrders request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("PlaceBatchLimitOrders unmarshal error: %v", err)
	}

	return result, nil
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
// PlaceLimitOrderForPeople places a limit order for a specific exchange and trade
func (s *SbeeRest) PlaceLimitOrderForPeople(Exchange, Trade string, orders interface{}) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceLimitOrderForPeople", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	// Convert orders to JSON
	ordersJSON, err := json.Marshal(orders)
	if err != nil {
		return nil, fmt.Errorf("PlaceLimitOrderForPeople json marshal error: %v", err)
	}

	response, err := s.makeRequest(url, "POST", headers, string(ordersJSON))
	if err != nil {
		return nil, fmt.Errorf("PlaceLimitOrderForPeople request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("PlaceLimitOrderForPeople unmarshal error: %v", err)
	}

	return result, nil
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
// PlaceMarketOrderForPeople places a market order for a specific exchange and trade
func (s *SbeeRest) PlaceMarketOrderForPeople(Exchange, Trade string, orders interface{}) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/%s/%s/PlaceMarketOrderForPeople", s.baseURL, Exchange, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	// Convert orders to JSON
	ordersJSON, err := json.Marshal(orders)
	if err != nil {
		return nil, fmt.Errorf("PlaceMarketOrderForPeople json marshal error: %v", err)
	}

	response, err := s.makeRequest(url, "POST", headers, string(ordersJSON))
	if err != nil {
		return nil, fmt.Errorf("PlaceMarketOrderForPeople request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("PlaceMarketOrderForPeople unmarshal error: %v", err)
	}

	return result, nil
}

/*
	https://doc.sbee.io/api/info-markets
	It provides information about the owned stock exchange and the service endpoints used in the exchange.
*/
// Markets retrieves market information
func (s *SbeeRest) Markets() (map[string]interface{}, error) {
	url := s.baseURL + "/Crypto/Info/Markets"
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, fmt.Errorf("Markets request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("Markets unmarshal error: %v", err)
	}

	return result, nil
}

/*
	https://doc.sbee.io/api/fintech
	It adjusts the value of currencies relative to each other.
*/
// MoneyPairValues retrieves money pair values
func (s *SbeeRest) MoneyPairValues() (map[string]interface{}, error) {
	url := s.baseURL + "/Fintech/MoneyPairValues"
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, fmt.Errorf("MoneyPairValues request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("MoneyPairValues unmarshal error: %v", err)
	}

	return result, nil
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
// MultiOrderBook retrieves multi-market order book
func (s *SbeeRest) MultiOrderBook(Trade string, data string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/MultiMarket/%s/OrderBook", s.baseURL, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, data)
	if err != nil {
		return nil, fmt.Errorf("MultiOrderBook request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("MultiOrderBook unmarshal error: %v", err)
	}

	return result, nil
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
// MultiRecentTrades retrieves multi-market recent trades
func (s *SbeeRest) MultiRecentTrades(Trade string, data string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/MultiMarket/%s/RecentTrades", s.baseURL, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, data)
	if err != nil {
		return nil, fmt.Errorf("MultiRecentTrades request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("MultiRecentTrades unmarshal error: %v", err)
	}

	return result, nil
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
// SteppedOrderBook retrieves stepped order book for multi-market
func (s *SbeeRest) SteppedOrderBook(Trade string, data string) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/MultiMarket/%s/SteppedOrderBook", s.baseURL, Trade)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
		"Content-Type":  "application/json-patch+json",
	}

	response, err := s.makeRequest(url, "POST", headers, data)
	if err != nil {
		return nil, fmt.Errorf("SteppedOrderBook request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("SteppedOrderBook unmarshal error: %v", err)
	}

	return result, nil
}

/*
	https://doc.sbee.io/api/news
	$language='en';
	$pageSize=20;
	$pageNumber=1;
	$data= $Exchange->News($language,  $pageSize, $pageNumber);
*/
// News retrieves a list of news based on language, page size, and page number
func (s *SbeeRest) News(language string, pageSize, pageNumber int) (map[string]interface{}, error) {
	url := fmt.Sprintf("%s/Crypto/News/List?language=%s&pageSize=%d&pageNumber=%d", s.baseURL, url.QueryEscape(language), pageSize, pageNumber)
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, fmt.Errorf("News request error: %v", err)
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, fmt.Errorf("News unmarshal error: %v", err)
	}

	return result, nil
}

/*
https://doc.sbee.io/api/country
Country
The "country" endpoint provides information about a specific country.
*/
func (s *SbeeRest) Country() (map[string]interface{}, map[string]interface{}) {
	url := s.baseURL + "/Crypto/Country/List"
	headers := map[string]string{
		"accept":        "text/plain",
		"Authorization": "Bearer " + s.auth,
	}

	response, err := s.makeRequest(url, "GET", headers, "")
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	var result map[string]interface{}
	err = json.Unmarshal(response, &result)
	if err != nil {
		return nil, map[string]interface{}{"ERROR": err.Error()}
	}

	return result, nil
}

func main() {
	// Örnek kullanım
	baseURL := "https://api.sbee.io/api"
	auth := "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiI4NiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJBZG1pbmlzdHJhdG9yIiwiVXNlciJdLCJuYmYiOjE2OTY3NjQyOTMsImV4cCI6MTk1NzQ0NTQ0Nn0.1x4j1POo8AmXjumymcGUouri-9mLpnFEmpbqFPo4QsQ"

	sbeeRest := &SbeeRest{baseURL: baseURL, auth: auth}

	// Country çağrısı
	countryResult, countryErr := sbeeRest.MoneyPairValues()
	if countryErr != nil {
		fmt.Println("Country Error:", countryErr)
		return
	}
	fmt.Println("Country Result:", countryResult)
}
