import xml.etree.ElementTree as ET

tree = ET.parse('Results_AllTests.xml')
root = tree.getroot()

failed_tests = root.findall('.//test-case[@result="Failed"]')
for test in failed_tests:
    name = test.get('fullname')
    msg = test.find('.//message')
    print(f"FAILED: {name}")
    if msg is not None:
        print(f"  {msg.text}")
