import requests
import urllib
import uuid
import json
import sys

def handle_response(response):
    asjson = response.json()
    return [item for item in asjson['items']]


def execute_request(base_id, items, base_url, headers):
    request = {'id': base_id, 'items': items}
    response = requests.post(
        base_url, data=json.dumps(request), headers=headers)
    handled_response = handle_response(response)
    base_id = str(uuid.uuid4())
    items.clear()
    return (handled_response, base_id, items)


def get_messages(directory):
    inputs = open(directory, encoding='utf8')
    messages = [x.strip().replace('"', '') for x in inputs if len(x) > 0]
    inputs.close()
    return messages


def process_messages(base_url, messages, amount=25):
    responses = []
    items = []
    i = 0
    base_id = str(uuid.uuid4())
    headers = {'content-type': 'application/json'}
    for msg in messages:
        item = {'id': base_id+'_'+str(i), 'text': msg, 'dateCheck': False, 'configuration': {'unidecodeNormalization': True,
                                                                                             'toLower': True, 'informationLevel': 3}}
        items.append(item)
        i += 1
        if (i % amount) == 0:
            r, base_id, items = execute_request(
                base_id, items, base_url, headers)
            print(str(i) + " processed")
            responses.extend(r)

    if len(items) > 0:
        r, base_id, items = execute_request(base_id, items, base_url, headers)
        print(str(i) + " processed")
        responses.extend(r)
    return responses


def write_responses(directory, responses):
    output = open(directory, encoding='utf8', mode='w')
    for r in responses:
        output.write(str(r) + "\n")
    output.close()

def analyse_response(responses):
    total_st = 0
    inputs_with_st = 0
    just_st = 0
    for r in responses:
        analysis = r['analysis']
        mc = int(analysis['matchesCount'])
        total_st += mc
        if mc > 0:
            inputs_with_st += 1
        uc = bool(analysis['useCleaned'])
        if not uc:
            just_st += 1
    return (total_st, inputs_with_st, just_st)

base_url = 'http://az-infobots.take.net/smalltalks/api/analysis/batch'

input_file = sys.argv[1]
output_file = sys.argv[2]
amount = int(sys.argv[3])

messages = get_messages(input_file)
responses = process_messages(base_url, messages, amount=amount)
write_responses(output_file, responses)
total_st, inputs_with_st, just_st = analyse_response(responses)

print('This file has {} smalltalks scattered in {} inputs. {} ({:.2f}%) inputs could be handle only with smalltalks.'.format(
      total_st, inputs_with_st, just_st, 100*float(just_st/float(len(messages)))))
