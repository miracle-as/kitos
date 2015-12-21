﻿beforeEach(() => {
    jasmine.addMatchers({
        "toMatchInRequests": (util: jasmine.MatchersUtil): jasmine.CustomMatcher => {
            // output a single request
            var outputRequest = (request: mock.ReceivedRequest) => {
                return "METHOD: " + request.method + " URL: " + request.url;
            }

            // output all requests
            var outputRequests = (requests: Array<mock.ReceivedRequest>) => {
                var output = "Actual requests: " + requests.length;
                if (requests.length > 0) {
                    output += "\n";
                    for (var i = 0; i < requests.length; i++) {
                        output += "  " + outputRequest(requests[i]);
                        if (i < requests.length - 1)
                            output += "\n";
                    }
                    output += "\n";
                }

                return output;
            }

            // compare two requests
            var compareRequest = (actual: mock.ReceivedRequest, expected: mock.ReceivedRequest) => {
                return actual.method === expected.method && actual.url.search(expected.url) !== -1;
            }

            var compare = (actual, expected) => {
                var result = {
                    pass: false,
                    message: null
                };

                result.message = util.buildFailureMessage("toMatchRequest", false, outputRequests(actual), "\n  " + outputRequest(expected));

                for (var i = 0; i < actual.length; i++) {
                    if (compareRequest(actual[i], expected)) {
                        result.pass = true;
                        break;
                    }
                }

                return result;
            };

            return {
                compare: compare
            };
        }
    });
});

declare module jasmine {
    interface Matchers {
        /**
         * returns true if expected request is in mocked requests
         *
         * @param expected A mock.ReceivedRequest object. URL is matched with regex.
         */
        toMatchInRequests(expected: mock.ReceivedRequest): boolean;
    }
}
