import xml.etree.ElementTree as ET

tree = ET.parse('Results_AllTests.xml')
root = tree.getroot()

failed_tests = root.findall('.//test-case[@result="Failed"]')
for test in failed_tests:
    name = test.get('fullname')
    if "CrashKonijn" in str(test.find('.//message').text) or "EnemyDecisionMaker" in name:
        msg = test.find('.//message')
        stack = test.find('.//stack-trace')
        print(f"FAILED: {name}")
        if msg is not None:
            print(f"  {msg.text}")
        if stack is not None:
            print(f"  {stack.text}")
